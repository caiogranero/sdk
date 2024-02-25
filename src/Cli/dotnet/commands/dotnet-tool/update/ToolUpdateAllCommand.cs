﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.Cli;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.ToolPackage;
using Microsoft.DotNet.Tools.Tool.Install;
using Microsoft.DotNet.Tools.Tool.List;

namespace Microsoft.DotNet.Tools.Tool.Update
{
    internal class ToolUpdateAllCommand : CommandBase
    {
        private readonly bool _global;

        public ToolUpdateAllCommand(ParseResult parseResult)
            : base(parseResult)
        {
            _global = parseResult.GetValue(ToolUpdateCommandParser.GlobalOption);
        }

        public override int Execute()
        {
            if (_global)
            {
                ToolUpdateAllGlobalCommand(_parseResult);
            }
            else
            {
                ToolUpdateAllLocalCommand(_parseResult);
            }
            return 0;
        }

        private int ToolUpdateAllGlobalCommand(ParseResult parseResult)
        {
            var toolListCommand = new ToolListGlobalOrToolPathCommand(parseResult);
            var toolList = toolListCommand.GetPackages(null, null);

            foreach (var tool in toolList)
            {
                string[] args= ["dotnet", "tool", "update", tool.Id.ToString(), "--global"];
                parseResult = Parser.Instance.Parse(args);
                var toolUpdateCommand = new ToolUpdateCommand(parseResult);
                toolUpdateCommand.Execute();
            }
            return 0;
        }

        private int ToolUpdateAllLocalCommand(ParseResult parseResult)
        {
            var toolListLocalCommand = new ToolListLocalCommand(parseResult);
            var toolListLocal = toolListLocalCommand.GetPackages(null);

            foreach (var (package, manifestPath) in toolListLocal)
            {
                var argsList = new List<string> { "dotnet", "tool", "update", package.PackageId.ToString(), "--local" };

                if (!string.IsNullOrEmpty(manifestPath.Value))
                {
                    argsList.Add("--tool-manifest");
                    argsList.Add(manifestPath.Value);
                }

                parseResult = Parser.Instance.Parse(argsList.ToArray());
                var toolUpdateCommand = new ToolUpdateCommand(parseResult);
                toolUpdateCommand.Execute();
            }
            return 0;
        }
    }
}
