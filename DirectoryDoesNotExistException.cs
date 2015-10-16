using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XslMail
{
	class DirectoryDoesNotExistException: Exception
	{
		public DirectoryDoesNotExistException(string path): 
			base(String.Format("Directory '{0}' does not exist.", path))
		{
		}
	}
}
