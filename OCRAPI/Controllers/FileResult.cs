using System.Collections.Generic;

namespace OCRAPI
{
    // This class is used to carry the result of various file uploads.
    public class FileResult
    {
        // Gets or sets the local path of the file saved on the server.
        public IEnumerable<string> FileNames { get; set; }

        // Text extracted with OCR.
        public string RecognizedText { get; set; }
    }
}
