using Microsoft.AspNetCore.Mvc;
using System.Xml;
using XmlValidationApp.Models;

namespace XmlValidationApp.Controllers
{
    public class XmlController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public XmlController( IWebHostEnvironment hostingEnvironment )
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ValidateXml( XmlUploadViewModel model )
        {
            if ( model.XmlFile == null || model.XmlFile.Length == 0 )
            {
                ModelState.AddModelError( "XmlFile", "Please upload an XML file." );
                return View( "Index" );
            }

            var settings   = new XmlReaderSettings();
            var schemaPath = Path.Combine( _hostingEnvironment.ContentRootPath, "Data", "note.xsd" );
            settings.Schemas.Add( null, schemaPath );
            settings.ValidationType = ValidationType.Schema;

            settings.ValidationEventHandler += ( sender, args ) =>
            {
                ModelState.AddModelError( "XmlFile", args.Message );
            };

            using ( var xmlReader = XmlReader.Create( model.XmlFile.OpenReadStream(), settings ) )
            {
                try
                {
                    while ( xmlReader.Read() ) { }
                }
                catch ( XmlException e )
                {
                    ModelState.AddModelError( "XmlFile", e.Message );
                }
            }

            if ( !ModelState.IsValid )
            {
                return View( "Index" );
            }

            return RedirectToAction( "Success" );
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
