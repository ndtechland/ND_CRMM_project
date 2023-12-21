using System.IO;
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using DinkToPdf;



namespace CRM.Repository
{
    public class PdfService
    {
        private readonly IConverter _converter;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PdfService(IConverter converter, IWebHostEnvironment hostingEnvironment)
        {
            _converter = converter;
            _hostingEnvironment = hostingEnvironment;
        }

        public byte[] GeneratePdf(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
                DPI = 300,
                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            };

            var document = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _converter.Convert(document);
        }

        public string GetAppPath()
        {
            return _hostingEnvironment.ContentRootPath;
        }

        public string GetAppBaseUrl()
        {
            return $"{_hostingEnvironment.WebRootPath}/";
        }

    }

}

