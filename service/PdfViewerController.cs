using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Syncfusion.Pdf;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Presentation;
using Syncfusion.PresentationRenderer;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using WFormatType = Syncfusion.DocIO.FormatType;


namespace PdfViewerWebService
{
    [Route("[controller]")]
    [ApiController]
    public class PdfViewerController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        //Initialize the memory cache object   
        public IMemoryCache _cache;
        public PdfViewerController(IWebHostEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
            Console.WriteLine("PdfViewerController initialized");
        }

        [HttpPost("LoadFile")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/LoadFile")]
        public IActionResult LoadFile([FromBody] Dictionary<string, string> jsonObject)
        {
            if (jsonObject.ContainsKey("data"))
            {

                string base64 = jsonObject["data"];
                //string fileName = args.FileData[0].Name; 
                string type = jsonObject["type"];
                string data = base64.Split(',')[1];
                byte[] bytes = Convert.FromBase64String(data);
                var outputStream = new MemoryStream();
                Syncfusion.Pdf.PdfDocument pdfDocument = new Syncfusion.Pdf.PdfDocument();
                using (Stream stream = new MemoryStream(bytes))
                {
                    switch (type)
                    {
                        case "docx":
                        case "dot":
                        case "doc":
                        case "dotx":
                        case "docm":
                        case "dotm":
                        case "rtf":
                            Syncfusion.DocIO.DLS.WordDocument doc = new Syncfusion.DocIO.DLS.WordDocument(stream, GetWFormatType(type));
                            //Initialization of DocIORenderer for Word to PDF conversion
                            DocIORenderer render = new DocIORenderer();
                            //Converts Word document into PDF document
                            pdfDocument = render.ConvertToPDF(doc);
                            doc.Close();
                            break;
                        case "pptx":
                        case "pptm":
                        case "potx":
                        case "potm":
                            //Loads or open an PowerPoint Presentation
                            IPresentation pptxDoc = Presentation.Open(stream);
                            pdfDocument = PresentationToPdfConverter.Convert(pptxDoc);
                            pptxDoc.Close();
                            break;
                        case "xlsx":
                        case "xls":
                            ExcelEngine excelEngine = new ExcelEngine();
                            //Loads or open an existing workbook through Open method of IWorkbooks
                            IWorkbook workbook = excelEngine.Excel.Workbooks.Open(stream);
                            //Initialize XlsIO renderer.
                            XlsIORenderer renderer = new XlsIORenderer();
                            //Convert Excel document into PDF document
                            pdfDocument = renderer.ConvertToPDF(workbook);
                            workbook.Close();
                            break;
                        case "jpeg":
                        case "jpg":
                        case "png":
                        case "bmp":
                            //Add a page to the document
                            PdfPage page = pdfDocument.Pages.Add();
                            //Create PDF graphics for the page
                            PdfGraphics graphics = page.Graphics;
                            PdfBitmap image = new PdfBitmap(stream);
                            //Draw the image
                            graphics.DrawImage(image, 0, 0);
                            break;
                        case "pdf":
                            string pdfBase64String = Convert.ToBase64String(bytes);
                            return Content("data:application/pdf;base64," + pdfBase64String);
                    }

                }
                pdfDocument.Save(outputStream);
                outputStream.Position = 0;
                byte[] byteArray = outputStream.ToArray();
                pdfDocument.Close();
                outputStream.Close();

                string base64String = Convert.ToBase64String(byteArray);
                return Content("data:application/pdf;base64," + base64String);


            }
            return Content("data:application/pdf;base64," + "");
        }
        public static WFormatType GetWFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("This is not a valid Word documnet.");
            switch (format.ToLower())
            {
                case "dotx":
                    return WFormatType.Dotx;
                case "docx":
                    return WFormatType.Docx;
                case "docm":
                    return WFormatType.Docm;
                case "dotm":
                    return WFormatType.Dotm;
                case "dot":
                    return WFormatType.Dot;
                case "doc":
                    return WFormatType.Doc;
                case "rtf":
                    return WFormatType.Rtf;
                default:
                    throw new NotSupportedException("This is not a valid Word documnet.");
            }
        }

        //GET api/values
       [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}