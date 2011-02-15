using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompiledViews.SharePoint
{
    /// <summary>
    /// Base class for action results. Behaviour is similar to ASP.NET MVC version, but greatly simplified.
    /// </summary>
    public abstract class ActionResult
    {
        public abstract void Execute();
    }

}
