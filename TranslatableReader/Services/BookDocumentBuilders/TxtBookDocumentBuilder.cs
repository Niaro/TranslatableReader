using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace TranslatableReader.Services
{
	public class TxtBookDocumentBuilder : BookDocumentBuilder, IBookDocumentBuilder
	{
		public List<Paragraph> Build(string bookOriginText)
		{
			var paragraphs = new List<Paragraph>();

			var txtRun = "";
			foreach (var oneChar in bookOriginText)
			{
				txtRun += oneChar;
				if (oneChar != 9 && oneChar != 10) continue;
				txtRun = new Regex("\\r\\n|\\r|\\n").Replace(txtRun, "");

				var newParagraph = new Paragraph()
				{
					Inlines = { new Run { Text = txtRun } }
				};
				txtRun = "";
				paragraphs.Add(newParagraph);
			}
			return paragraphs;
		}
	}
}
