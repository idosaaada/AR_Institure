using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System.Collections;

public class CameraCaptureAndEmail : MonoBehaviour
{
    // UI components
    public RawImage cameraDisplay;         // UI component to display camera feed
    public RawImage frameImage;            // UI component to overlay the frame on the photo
    public TMP_InputField emailInput;      // Input field for user to enter their email address
    public TextMeshProUGUI statusMessage;  // UI component to display messages to the user
    public Button captureButton;           // Button to capture a photo
    public Button sendEmailButton;         // Button to send the email with the photo
    public Button retakeButton;            // Button to retake the photo
    public Button switchCameraButton;      // Button to switch between front and back cameras

    // Camera-related variables
    private WebCamTexture webCamTexture;   // WebCamTexture to access the camera feed
    private Texture2D capturedImage;       // Texture2D to store the captured image
    private byte[] imageBytes;             // Byte array to hold the image data for emailing
    private int currentCameraIndex = 0;    // To track and switch between front and back cameras

    void Start()
    {
        // Request camera permission on Android
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
        }

        StartCamera(currentCameraIndex); // Start the default camera (usually the rear camera)

        // Set up button listeners
        captureButton.onClick.AddListener(CapturePhoto);      // Capture photo when capture button is clicked
        sendEmailButton.onClick.AddListener(SendEmailWithPhoto); // Send email when send email button is clicked
        retakeButton.onClick.AddListener(RetakePhoto);        // Retake the photo when retake button is clicked
        switchCameraButton.onClick.AddListener(SwitchCamera); // Switch camera when switch camera button is clicked
    }

    // Start the selected camera
    private void StartCamera(int cameraIndex)
    {
        // Check if there are any cameras available
        if (WebCamTexture.devices.Length > 0)
        {
            WebCamDevice selectedCamera = WebCamTexture.devices[cameraIndex];  // Select the camera
            webCamTexture = new WebCamTexture(selectedCamera.name);            // Assign camera feed to WebCamTexture
            cameraDisplay.texture = webCamTexture;                             // Display camera feed in UI
            webCamTexture.Play();                                              // Start the camera

            // Add this line to handle camera rotation
            StartCoroutine(RotateCamera());
        }
        else
        {
            statusMessage.text = "No camera available!";                       // Display an error if no camera found
        }
    }

    // Coroutine to handle camera rotation and flipping
    private IEnumerator RotateCamera()
    {
        yield return new WaitForEndOfFrame();

        // Rotate and flip the camera feed
        cameraDisplay.rectTransform.localRotation = Quaternion.Euler(0, 0, -webCamTexture.videoRotationAngle);

        // Flip the image if it's from the front camera (usually the second camera)
        bool isFrontFacing = WebCamTexture.devices[currentCameraIndex].isFrontFacing;
        cameraDisplay.rectTransform.localScale = new Vector3(isFrontFacing ? -1 : 1, 1, 1);

        // Adjust RawImage's size to match the aspect ratio of the camera feed
        float aspect = (float)webCamTexture.width / (float)webCamTexture.height;
        cameraDisplay.rectTransform.sizeDelta = new Vector2(cameraDisplay.rectTransform.sizeDelta.y * aspect, cameraDisplay.rectTransform.sizeDelta.y);

        // Ensure the frame matches the camera display size
        if (frameImage != null)
        {
            frameImage.rectTransform.sizeDelta = cameraDisplay.rectTransform.sizeDelta;  // Match frame size to camera display
        }
    }

    // Capture the photo and stop the camera feed
    public void CapturePhoto()
    {
        SoundManager.instance.PlaySound("camera shutter"); // Play camera shutter sound for feedback

        webCamTexture.Pause();  // Pause the camera after capturing the image

        // Create a correctly oriented Texture2D
        capturedImage = new Texture2D(webCamTexture.width, webCamTexture.height);
        Color[] pixels = webCamTexture.GetPixels();

        // Flip the image vertically
        for (int y = 0; y < webCamTexture.height; y++)
        {
            for (int x = 0; x < webCamTexture.width; x++)
            {
                capturedImage.SetPixel(x, webCamTexture.height - 1 - y, pixels[y * webCamTexture.width + x]);
            }
        }
        capturedImage.Apply();

        // Combine the captured image with the frame overlay (if available)
        CombineImageWithFrame();

        // Convert the image to PNG byte array to prepare for email attachment
        imageBytes = capturedImage.EncodeToPNG();

        statusMessage.text = "Photo captured! Ready to send.";
    }

    // Combine the captured image with the frame (if a frame is set)
    private void CombineImageWithFrame()
    {
        if (frameImage.texture != null)
        {
            // Convert the frame texture into a Texture2D and resize it to match the captured image
            Texture2D frameTexture = frameImage.texture as Texture2D;
            frameTexture = ResizeTexture(frameTexture, capturedImage.width, capturedImage.height);

            // Loop through all pixels in the captured image and overlay the frame
            for (int x = 0; x < capturedImage.width; x++)
            {
                for (int y = 0; y < capturedImage.height; y++)
                {
                    Color framePixel = frameTexture.GetPixel(x, y);
                    // Overlay only non-transparent pixels (alpha > 0.5)
                    if (framePixel.a > 0.5f)
                    {
                        capturedImage.SetPixel(x, y, framePixel);
                    }
                }
            }
            capturedImage.Apply(); // Apply the changes to the captured image
        }
    }

    // Resize the frame texture to match the size of the captured image
    private Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight); // Create a new texture with the target size
        float incX = 1.0f / targetWidth;   // Calculate horizontal scaling factor
        float incY = 1.0f / targetHeight;  // Calculate vertical scaling factor

        // Loop through pixels and resize the source texture
        for (int px = 0; px < result.width * result.height; px++)
        {
            result.SetPixel(px % targetWidth, px / targetWidth,
                source.GetPixelBilinear(incX * (px % targetWidth), incY * (px / targetWidth)));
        }
        result.Apply(); // Apply changes
        return result;  // Return the resized texture
    }

    // Send an email with the captured photo as an attachment
    public void SendEmailWithPhoto()
    {
        SoundManager.instance.PlaySound("select click"); // Play feedback sound on button press

        // Validate if the email is valid
        if (string.IsNullOrEmpty(emailInput.text) || !IsValidEmail(emailInput.text))
        {
            statusMessage.text = "Invalid email!";
            return;
        }

        try
        {
            // Create the email message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("arinstitureapp@gmail.com");    // Sender's email
            mail.To.Add(emailInput.text);                              // Recipient's email
            mail.Subject = "Your Photo from ARinstiture App";          // Subject of the email
            mail.Body = "Thank you for using the app! Here's your photo.";  // Body of the email

            // Attach the photo to the email if available
            if (imageBytes != null)
            {
                Attachment attachment = new Attachment(new MemoryStream(imageBytes), "CapturedPhoto.png");
                mail.Attachments.Add(attachment);  // Add the attachment to the email
            }
            else
            {
                statusMessage.text = "No photo to send!";
                return;
            }

            // Set up the SMTP server for sending the email
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential("arinstitureapp@gmail.com", "fgle eauw nseo iekr") as ICredentialsByHost;
            smtpServer.EnableSsl = true;
            smtpServer.Send(mail);  // Send the email

            statusMessage.text = "Email sent!";
        }
        catch (Exception ex)
        {
            statusMessage.text = "Failed to send email.";  // Error handling for email sending
            Debug.LogError(ex.Message);
        }
    }

    // Allow the user to retake the photo
    public void RetakePhoto()
    {
        SoundManager.instance.PlaySound("select click"); // Play feedback sound on button press
        capturedImage = null;       // Clear the previous image
        webCamTexture.Play();       // Restart the camera feed
        statusMessage.text = "You can capture a new photo.";
    }

    // Switch between the front and back cameras
    private void SwitchCamera()
    {
        SoundManager.instance.PlaySound("select click"); // Play feedback sound on button press

        // Stop the current camera and switch to the other one
        webCamTexture.Stop();
        currentCameraIndex = (currentCameraIndex + 1) % WebCamTexture.devices.Length;
        StartCamera(currentCameraIndex);  // Start the selected camera
    }

    // Validate if the input email address has a valid format
    private bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Address == email;  // Return true if the email format is valid
        }
        catch
        {
            return false;  // Return false if an exception occurs
        }
    }
}
