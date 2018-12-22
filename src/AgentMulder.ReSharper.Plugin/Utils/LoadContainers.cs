using System;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace AgentMulder.ReSharper.Plugin.Utils
{
    public static class LoadContainers
    {
        public static CompositionContainer LoadContainersDll()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Debug.Assert(path != null, "path != null");
            path = Path.Combine(path, "Containers");

            if (!Directory.Exists(path))
            {
                return null;
            }

            var catalog = new DirectoryCatalog(path, "*.dll");
            return new CompositionContainer(catalog);
        }
    }
}
