using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XslMail
{
	class DirectoryIsEmptyException: Exception
	{
		public DirectoryIsEmptyException(string path): 
			base(String.Format("Directory '{0}' is missing expected files or subfolders.", path))
		{
		}
	}
}
