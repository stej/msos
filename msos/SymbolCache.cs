﻿using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.RuntimeExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msos
{
    class SymbolCache
    {
        private HashSet<string> _failedToLoadSymbols = new HashSet<string>();

        public Microsoft.Diagnostics.RuntimeExt.SourceLocation GetFileAndLineNumberSafe(ClrStackFrame frame)
        {
            if (frame.Method == null || frame.Method.Type == null || frame.Method.Type.Module == null)
                return null;

            string moduleName = frame.Method.Type.Module.Name;
            if (_failedToLoadSymbols.Contains(moduleName))
                return null;

            // ClrStackFrame.GetFileAndLineNumber throws occasionally when something is wrong with the module.
            try
            {
                var location = frame.GetSourceLocation();
                if (location == null)
                    _failedToLoadSymbols.Add(moduleName);
                
                return location;
            }
            catch (ClrDiagnosticsException)
            {
                _failedToLoadSymbols.Add(moduleName);
                return null;
            }
            catch (InvalidOperationException)
            {
                _failedToLoadSymbols.Add(moduleName);
                return null;
            }
        }
    }
}
