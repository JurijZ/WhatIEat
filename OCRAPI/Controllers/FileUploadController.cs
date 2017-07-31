using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Tesseract;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace OCRAPI
{
    /// <summary>
    /// This sample controller reads the contents of an HTML file upload asynchronously and writes one or more body parts to a local file.
    /// </summary>
    public class FileUploadController : ApiController
    {
        static readonly string ServerUploadFolder = "D:\\WhatIEat"; //Path.GetTempPath();

        [HttpPost]
        public async Task<string> UploadFile()
        {
            // Verify that this is an HTML Form file upload request
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
            }

            // Create a stream provider for setting up output streams
            MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(ServerUploadFolder);

            // Read the MIME multipart asynchronously content using the stream provider we just created.
            await Request.Content.ReadAsMultipartAsync(streamProvider);

            //--------------------------------------------------------------
            // OCR
            //--------------------------------------------------------------

            // Get the uploaded file path
            var testImagePath = streamProvider.FileData.Select(entry => entry.LocalFileName).FirstOrDefault();
            Console.WriteLine("File path: {0}", testImagePath);

            // Create an object to be returned
            var Results = new FileResult();
            Results.FileNames = streamProvider.FileData.Select(entry => entry.LocalFileName);
            Results.RecognizedText = "";

            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            //var text = page.GetText();
                            Results.RecognizedText = page.GetText();
                            Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

                            Console.WriteLine("Text (GetText): \r\n{0}", Results.RecognizedText);

                            // Use this part to process text word by word:

                            //Console.WriteLine("Text (iterator):");
                            //using (var iter = page.GetIterator())
                            //{
                            //    iter.Begin();

                            //    do
                            //    {
                            //        do
                            //        {
                            //            do
                            //            {
                            //                do
                            //                {
                            //                    if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                            //                    {
                            //                        Console.WriteLine("<BLOCK>");
                            //                    }

                            //                    Console.Write(iter.GetText(PageIteratorLevel.Word));
                            //                    Console.Write(" ");

                            //                    if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                            //                    {
                            //                        Console.WriteLine();
                            //                    }
                            //                } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                            //                if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                            //                {
                            //                    Console.WriteLine();
                            //                }
                            //            } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                            //        } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                            //    } while (iter.Next(PageIteratorLevel.Block));
                            //}
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
            }

            // Create response
            return Results.RecognizedText;
            //return new FileResult
            //{
            //    FileNames = streamProvider.FileData.Select(entry => entry.LocalFileName),
            //    RecognizedText = recognizedTextOCR
            //    //RecognizedText = streamProvider.FormData["submitter"]
            //};
        }
    }
}
