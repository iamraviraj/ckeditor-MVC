using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ckeditorMVC
{
    public class CKEditorMVC
    {
        public static MvcHtmlString RichTextEditor(string EditorName, string EditorText = "", string cssName = @"")
        {
            TagBuilder tag = new TagBuilder("textarea");
            tag.MergeAttribute("name", TagBuilder.CreateSanitizedId(EditorName));//This will remove spaces and restricted characters.
            tag.MergeAttribute("rows", "10");//This will remove spaces and restricted characters.
            tag.MergeAttribute("style", "width:100%");//This will remove spaces and restricted characters.
            tag.GenerateId("editor");// This will generate ID for button based on Name property.
            tag.InnerHtml = EditorText; // To set Button Text
            tag.AddCssClass(cssName);// To set CSS Class attribute

            string assemblyPath = Path.GetDirectoryName(HostingEnvironment.MapPath("~/"));
            string filePathRelativeToAssembly = Path.Combine(assemblyPath, typeof(CKEditorMVC).Namespace + ".ckeditor.zip");
            string normalizedPath = Path.GetFullPath(filePathRelativeToAssembly);

            if (!File.Exists(filePathRelativeToAssembly))
            {
                var embeddedResourcesPath = Assembly.GetExecutingAssembly().GetManifestResourceNames();// This will give you list of file paths.
                ExtractEmbeddedResource(assemblyPath, Assembly.GetExecutingAssembly().Location, embeddedResourcesPath);

                ZipFile.ExtractToDirectory(normalizedPath, HostingEnvironment.MapPath("~/"));
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(tag.ToString());
            sb.Append("<script type=\"text/javascript\" src=\"/ckeditor/ckeditor.js\"></script>");
            sb.Append("<script type=\"text/javascript\" src=\"/ckeditor/sample.js\"></script>");
            sb.Append("<script type=\"text/javascript\">initSample();</script>");
            return MvcHtmlString.Create(sb.ToString());
        }

        /// <summary>
        /// Creates a new file using stream
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="resourceLocation"></param>
        /// <param name="files"></param>
        private static void ExtractEmbeddedResource(string outputDir, string resourceLocation, string[] files)
        {
            foreach (string file in files)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file))
                {
                    using (FileStream fileStream = new FileStream(Path.Combine(outputDir, file), FileMode.Create))
                    {
                        for (int i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }
                        fileStream.Close();
                    }
                }
            }
        }
    }
}
