﻿using CsScriptManaged.SymbolProviders;
using CsScriptManaged.Utility;
using DbgEngManaged;
using System;
using System.IO;
using System.Reflection;

namespace CsScriptManaged
{
    /// <summary>
    /// Static class that has the whole debugging context
    /// </summary>
    public static class Context
    {
        /// <summary>
        /// The DbgEng.dll Advanced interface
        /// </summary>
        public static IDebugAdvanced3 Advanced;

        /// <summary>
        /// The DbgEng.dll Client interface
        /// </summary>
        public static IDebugClient7 Client;

        /// <summary>
        /// The DbgEng.dll Control interface
        /// </summary>
        public static IDebugControl7 Control;

        /// <summary>
        /// The DbgEng.dll Data spaces interface
        /// </summary>
        public static IDebugDataSpaces4 DataSpaces;

        /// <summary>
        /// The DbgEng.dll Registers interface
        /// </summary>
        public static IDebugRegisters2 Registers;

        /// <summary>
        /// The DbgEng.dll Symbols interface
        /// </summary>
        public static IDebugSymbols5 Symbols;

        /// <summary>
        /// The DbgEng.dll System objects interface
        /// </summary>
        public static IDebugSystemObjects4 SystemObjects;

        /// <summary>
        /// The symbol provider interface
        /// </summary>
        public static ISymbolProvider SymbolProvider;

        /// <summary>
        /// The DbgEng.dll symbol provider
        /// </summary>
        private static DbgEngSymbolProvider DbgEngSymbolProvider = new DbgEngSymbolProvider();

        /// <summary>
        /// The DIA symbol provider
        /// </summary>
        private static DiaSymbolProvider DiaSymbolProvider = new DiaSymbolProvider();

        /// <summary>
        /// The user type metadata (used for casting to user types)
        /// </summary>
        internal static UserTypeMetadata[] UserTypeMetadata;

        /// <summary>
        /// The settings for script execution
        /// </summary>
        internal static Settings Settings = new Settings();

        /// <summary>
        /// The interactive execution
        /// </summary>
        private static InteractiveExecution interactiveExecution = new InteractiveExecution();

        /// <summary>
        /// The state cache
        /// </summary>
        internal static StateCache StateCache = new StateCache();

        /// <summary>
        /// Gets or sets a value indicating whether variable caching is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if variable caching is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool EnableVariableCaching { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user casted variable caching is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if user casted variable caching is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool EnableUserCastedVariableCaching { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether variable path tracking is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if variable path tracking is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool EnableVariablePathTracking { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether debugger is currently in live debugging.
        /// </summary>
        /// <value>
        /// <c>true</c> if debugger is currently in live debugging; otherwise, <c>false</c>.
        /// </value>
        public static bool IsLiveDebugging
        {
            get
            {
                try
                {
                    return Client.GetNumberDumpFiles() == 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Initalizes the Context with the specified DbgEng.dll Client interface.
        /// </summary>
        /// <param name="client">The DbgEng.dll Client interface.</param>
        public static void Initalize(IDebugClient client)
        {
            Advanced = client as IDebugAdvanced3;
            Client = client as IDebugClient7;
            Control = client as IDebugControl7;
            DataSpaces = client as IDebugDataSpaces4;
            Registers = client as IDebugRegisters2;
            Symbols = client as IDebugSymbols5;
            SystemObjects = client as IDebugSystemObjects4;
            SymbolProvider = DbgEngSymbolProvider;
            SymbolProvider = DiaSymbolProvider;
            StateCache = new StateCache();
        }

        /// <summary>
        /// Executes the specified script.
        /// </summary>
        /// <param name="path">The script path.</param>
        /// <param name="args">The arguments.</param>
        public static void Execute(string path, params string[] args)
        {
            ExecuteAction(() =>
            {
                using (ScriptExecution execution = new ScriptExecution())
                {
                    execution.Execute(path, args);
                }
            });
        }

        /// <summary>
        /// Enters the interactive mode.
        /// </summary>
        public static void EnterInteractiveMode()
        {
            ExecuteAction(() => interactiveExecution.Run());
        }

        /// <summary>
        /// Interprets the C# code.
        /// </summary>
        /// <param name="code">The C# code.</param>
        public static void Interpret(string code)
        {
            ExecuteAction(() => interactiveExecution.Interpret(code));
        }

        /// <summary>
        /// Clears the metadata cache.
        /// </summary>
        internal static void ClearMetadataCache()
        {
            // Clear metadata from processes
            foreach (var process in GlobalCache.Processes.Values)
            {
                process.ClearMetadataCache();
            }

            // Clear user types metadata
            UserTypeMetadata = new UserTypeMetadata[0];
            foreach (var cacheEntry in GlobalCache.VariablesUserTypeCastedFields)
            {
                cacheEntry.Cached = false;
            }

            foreach (var cacheEntry in GlobalCache.VariablesUserTypeCastedFieldsByName)
            {
                cacheEntry.Clear();
            }

            foreach (var cacheEntry in GlobalCache.UserTypeCastedVariableCollections)
            {
                cacheEntry.Cached = false;
            }

            foreach (var cacheEntry in GlobalCache.UserTypeCastedVariables)
            {
                cacheEntry.Clear();
            }

            GlobalCache.VariablesUserTypeCastedFields.Clear();
            GlobalCache.VariablesUserTypeCastedFieldsByName.Clear();
            GlobalCache.UserTypeCastedVariableCollections.Clear();
        }

        /// <summary>
        /// Executes the action in redirected console output and error stream.
        /// </summary>
        /// <param name="action">The action.</param>
        private static void ExecuteAction(Action action)
        {
            TextWriter originalConsoleOut = Console.Out;
            TextWriter originalConsoleError = Console.Error;

            Console.SetOut(new DebuggerTextWriter(DebugOutput.Normal));
            Console.SetError(new DebuggerTextWriter(DebugOutput.Error));
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            finally
            {
                Console.SetOut(originalConsoleOut);
                Console.SetError(originalConsoleError);
                StateCache.SyncState();
            }
        }

        /// <summary>
        /// Gets the assembly directory.
        /// </summary>
        internal static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);

            path = Path.GetDirectoryName(path);
            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }

            return path;
        }
    }
}
