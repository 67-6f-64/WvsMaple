using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WvsGame.Net;

namespace WvsGame.Maple.Scripting
{
    public static class ScriptingFactory
    {
        private static CodeDomProvider _compiler;
        private static CompilerParameters _compilerParams;

        public static void Initialize()
        {
            foreach (ScriptType type in (ScriptType[])Enum.GetValues(typeof(ScriptType)))
            {
                GameServer.Scripts.Add(type, new Dictionary<string, Type>());
            }

            CompileScripts(ScriptType.Npc);
            CompileScripts(ScriptType.Portal);
        }

        public static void CompileScripts(ScriptType type)
        {
            Dictionary<string, Type> usableScripts = new Dictionary<string, Type>();
            Dictionary<string, DateTime> compiledScripts = new Dictionary<string, DateTime>();

            foreach (var script in System.IO.Directory.GetFiles("..\\DataSvr\\Script\\" + type.ToString() + "\\Compiled\\", "*.compiled"))
            {
                compiledScripts.Add(System.IO.Path.GetFileNameWithoutExtension(script), System.IO.File.GetLastWriteTime(script));
            }

            List<string> needCompliation = new List<string>();
            foreach (var script in System.IO.Directory.GetFiles("..\\DataSvr\\Script\\" + type.ToString() + "\\", "*.cs"))
            {
                var shortname = System.IO.Path.GetFileNameWithoutExtension(script);
                if (!compiledScripts.ContainsKey(shortname) || compiledScripts[shortname] < System.IO.File.GetLastWriteTime(script))
                {
                    needCompliation.Add(script);
                }
            }

            foreach (var script in needCompliation)
            {
                var shortName = Path.GetFileNameWithoutExtension(script);

                if (usableScripts.ContainsKey(shortName))
                {
                    usableScripts.Remove(shortName);
                }

                CompilerResults results = ScriptingFactory.CompileScript(type, script);

                if (results.Errors.Count > 0)
                {
                    MainForm.Instance.Log("");
                    MainForm.Instance.Log("Error compiling " +  type.ToString() + " Script '{0}'.", shortName);
                    MainForm.Instance.Log("");

                    foreach (CompilerError error in results.Errors)
                    {
                        MainForm.Instance.Log("");
                        MainForm.Instance.Log("Line '{0}', Column '{1}': {2}", error.Line, error.Column, error.ErrorText);
                    }
                }
                else
                {
                    MainForm.Instance.Log("Compiled " + type.ToString() + " '{0}'.", shortName);
                }
            }

            foreach (var script in System.IO.Directory.GetFiles("..\\DataSvr\\Script\\" + type.ToString() + "\\Compiled\\", "*.compiled"))
            {
                var shortName = System.IO.Path.GetFileNameWithoutExtension(script);

                if (!usableScripts.ContainsKey(shortName))
                {
                    Assembly assembly = Assembly.Load(File.ReadAllBytes(Environment.CurrentDirectory + "\\" + script));
                    Type[] types = assembly.GetTypes();

                    foreach (Type assemblyType in types)
                    {
                        if (type == ScriptType.Npc)
                        {
                            if (!assemblyType.IsSubclassOf(typeof(NpcScript)))
                            {
                                continue;
                            }
                        }
                        else if (type == ScriptType.Portal)
                        {
                            if (!assemblyType.IsSubclassOf(typeof(PortalScript)))
                            {
                                continue;
                            }
                        }

                        AddScript(type, assemblyType, shortName);
                    }
                }
            }
        }

        private static void AddScript(ScriptType type, Type assemblyType, string label)
        {
            switch (type)
            {
                case ScriptType.Npc:
                    {
                        if (!GameServer.Scripts[ScriptType.Npc].ContainsKey(label))
                        {
                            GameServer.Scripts[ScriptType.Npc].Add(label, assemblyType);
                        }
                    }
                    break;

                case ScriptType.Portal:
                    {
                        if (!GameServer.Scripts[ScriptType.Portal].ContainsKey(label))
                        {
                            GameServer.Scripts[ScriptType.Portal].Add(label, assemblyType);
                        }
                    }
                    break;
            }
        }

        public static CompilerResults CompileScript(ScriptType type, string pSource)
        {
            try
            {
                if (_compiler == null)
                {
                    _compiler = CodeDomProvider.CreateProvider("CSharp");
                    _compilerParams = new CompilerParameters();

                    _compilerParams.CompilerOptions = "/optimize";
                    _compilerParams.GenerateInMemory = true;
                    _compilerParams.GenerateExecutable = false;
                    _compilerParams.IncludeDebugInformation = false;

                    foreach (AssemblyName reference in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                    {
                        _compilerParams.ReferencedAssemblies.Add(reference.Name + ".dll");
                    }

                    _compilerParams.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
                }

                _compilerParams.OutputAssembly = "..\\DataSvr\\Script\\" + type.ToString() + "\\Compiled\\" + System.IO.Path.GetFileNameWithoutExtension(pSource) + ".compiled";
                var result = _compiler.CompileAssemblyFromFile(_compilerParams, pSource);

                if (System.IO.File.Exists(_compilerParams.OutputAssembly))
                {
                    System.IO.File.SetLastWriteTime(_compilerParams.OutputAssembly, System.IO.File.GetLastWriteTime(pSource));
                }

                _compiler.Dispose();

                return result;
            }
            catch (Exception e)
            {
                MainForm.Instance.Log(e.ToString());
                return null;
            }
        }
    }

    public enum ScriptType
    {
        Npc,
        Portal
    }
}
