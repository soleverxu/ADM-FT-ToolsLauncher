﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    class ArgHelper
    {
        private const string ShortFormArgIndicator = "-";
        private const string LongFormArgIndicator = "--";

        private static readonly ArgHelper _instance = new ArgHelper();

        private List<OptArgInfo> _optArgs;
        private List<PosArgInfo> _posArgs;

        public static ArgHelper Instance { get { return _instance; } }

        static ArgHelper() { }

        private ArgHelper()
        {
            _optArgs = new List<OptArgInfo>();
            _posArgs = new List<PosArgInfo>();

            Init();
        }

        private void Init()
        {
            Type t = typeof(CommandArguments);
            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                ArgDescriptionAttribute argDesc = pi.GetCustomAttribute<ArgDescriptionAttribute>();
                IEnumerable<ArgSampleAttribute> argSamples = pi.GetCustomAttributes<ArgSampleAttribute>();

                OptionalArgAttribute optArg = pi.GetCustomAttribute<OptionalArgAttribute>();
                if (optArg != null)
                {
                    _optArgs.Add(new OptArgInfo(pi, optArg)
                    {
                        Value = pi.GetCustomAttribute<OptionalArgValueAttribute>(),
                        Description = argDesc,
                        Samples = argSamples
                    });
                    continue;
                }

                PositionalArgAttribute posArg = pi.GetCustomAttribute<PositionalArgAttribute>();
                if (posArg != null)
                {
                    _posArgs.Add(new PosArgInfo(pi, posArg)
                    {
                        Description = argDesc,
                        Samples = argSamples
                    });
                    continue;
                }
            }

            _posArgs.Sort((x, y) =>
            {
                if (x == null && y == null) return 0;
                else if (x == null) return -1;
                else if (y == null) return 1;
                else return x.Argument.Position - y.Argument.Position;
            });
        }

        private bool IsOptionalArgument(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg) || arg.Length < 2)
            {
                return false;
            }

            if (arg.StartsWith(ShortFormArgIndicator) || arg.StartsWith(LongFormArgIndicator))
            {
                return true;
            }

            return false;
        }

        private bool IsOptionalArgument(string arg, IEnumerable<string> argNames)
        {
            foreach (string name in argNames)
            {
                if (name.Length == 1)
                {
                    if (ShortFormArgIndicator + name == arg)
                    {
                        return true;
                    }
                }
                else if (name.Length > 1)
                {
                    if (LongFormArgIndicator + name == arg)
                    {
                        return true;
                    }
                    else if (ShortFormArgIndicator + name == arg)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public CommandArguments ParseCommandArguments(string[] args, out string[] errors)
        {
            List<string> errorList = new List<string>();

            CommandArguments cmdArgs = new CommandArguments();

            int pos = -1;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (IsOptionalArgument(arg))
                {
                    // optional argument
                    bool isRecognized = false;
                    foreach (OptArgInfo optArg in _optArgs)
                    {
                        if (IsOptionalArgument(arg, optArg.Argument.Names))
                        {
                            isRecognized = true;
                            Type propertyType = optArg.PropertyInfo.PropertyType;
                            if (propertyType == typeof(bool))
                            {
                                optArg.PropertyInfo.SetValue(cmdArgs, true);
                            }
                            else if (propertyType == typeof(string))
                            {
                                i++;
                                if (i >= args.Length)
                                {
                                    errorList.Add(string.Format(Properties.Resources.ErrorMsg_MissingOptionalArgValue, optArg.Argument.FirstName));
                                }
                                else
                                {
                                    optArg.PropertyInfo.SetValue(cmdArgs, args[i]);
                                }
                            }
                            else if (propertyType == typeof(int))
                            {
                                i++;
                                if (i >= args.Length)
                                {
                                    errorList.Add(string.Format(Properties.Resources.ErrorMsg_MissingOptionalArgValue, optArg.Argument.FirstName));
                                }
                                else
                                {
                                    int intValue = 0;
                                    if (!int.TryParse(args[i], out intValue))
                                    {
                                        errorList.Add(string.Format(Properties.Resources.ErrorMsg_InvalidOptionalArgValue, optArg.Argument.FirstName));
                                    }
                                    else
                                    {
                                        optArg.PropertyInfo.SetValue(cmdArgs, intValue);
                                    }
                                }
                            }
                            else
                            {
                                // not supported property type
                                errorList.Add(string.Format(Properties.Resources.ErrorMsg_UnknownArgPropertyType, optArg.Argument.FirstName, propertyType.FullName));
                            }

                            break;
                        }
                    }

                    if (!isRecognized)
                    {
                        errorList.Add(string.Format(Properties.Resources.WarningMsg_UnknownOption, arg));
                    }
                }
                else
                {
                    // positional argument
                    cmdArgs.AllPositionalArgs.Add(arg);

                    pos++;
                    if (pos < _posArgs.Count)
                    {
                        PosArgInfo posArg = _posArgs[pos];
                        posArg.PropertyInfo.SetValue(cmdArgs, arg);
                    }
                }
            }

            errors = errorList.ToArray();
            return cmdArgs;
        }

        public void WriteUsage(TextWriter w, string programName = "")
        {
            if (w == null)
            {
                return;
            }

            // one-line usage
            if (string.IsNullOrWhiteSpace(programName))
            {
                programName = Assembly.GetEntryAssembly().GetName().Name;
            }

            string argsUsageLine = string.Empty;
            if (_optArgs.Count > 0)
            {
                argsUsageLine += " " + Properties.Resources.Prog_Usage_OptionsSection;
            }
            foreach (PosArgInfo posArg in _posArgs)
            {
                argsUsageLine += " " + posArg.Argument.PlaceholderText;
            }

            w.WriteLine(Properties.Resources.Prog_Usage_OneLine, programName + argsUsageLine);
            w.WriteLine();

            const string indentLv1 = "  ";
            const string indentLv2 = "    ";

            // positional argument details
            if (_posArgs.Count > 0)
            {
                foreach (PosArgInfo posArg in _posArgs)
                {
                    if (posArg.Description != null && posArg.Description.Lines.Count() > 0)
                    {
                        w.WriteLine(posArg.Argument.PlaceholderText);
                        foreach (string line in posArg.Description.Lines)
                        {
                            w.WriteLine(indentLv1 + line);
                        }
                        w.WriteLine();
                    }

                    if (posArg.Samples != null)
                    {
                        int i = 0;
                        foreach (ArgSampleAttribute sample in posArg.Samples)
                        {
                            if (sample.Lines.Count() > 0)
                            {
                                i++;
                                w.WriteLine(string.Format(indentLv1 + Properties.Resources.Prog_Usage_SampleTitle, i));
                                foreach (string line in sample.Lines)
                                {
                                    w.WriteLine(indentLv1 + line);
                                }
                                w.WriteLine();
                            }
                        }
                    }
                }
                w.WriteLine();
            }

            // optional argument details
            if (_optArgs.Count > 0)
            {
                w.WriteLine(Properties.Resources.Prog_Usage_OptArgsTitle);
                w.WriteLine();

                foreach (OptArgInfo optArg in _optArgs)
                {
                    // arg name(s)
                    List<string> shortFormList = new List<string>();
                    List<string> longFormList = new List<string>();
                    foreach (string name in optArg.Argument.Names)
                    {
                        if (name.Length == 1)
                        {
                            shortFormList.Add(ShortFormArgIndicator + name);
                        }
                        else
                        {
                            longFormList.Add(LongFormArgIndicator + name);
                        }
                    }
                    w.Write(indentLv1);
                    if (shortFormList.Count > 0)
                    {
                        w.Write(string.Join<string>(", ", shortFormList));
                    }
                    if (longFormList.Count > 0)
                    {
                        if (shortFormList.Count > 0)
                        {
                            w.Write(", ");
                        }
                        w.Write(string.Join<string>(", ", longFormList));
                    }

                    // arg value
                    if (optArg.Value != null)
                    {
                        w.Write(" ");
                        w.Write(optArg.Value.PlaceholderText);
                    }

                    // required arg?
                    if (optArg.Argument.Required)
                    {
                        w.Write("  " + Properties.Resources.Prog_Usage_MandatoryOption);
                    }
                    w.WriteLine();

                    // arg description
                    if (optArg.Description != null)
                    {
                        foreach (string line in optArg.Description.Lines)
                        {
                            w.WriteLine(indentLv2 + line);
                        }
                        w.WriteLine();
                    }

                    if (optArg.Samples != null)
                    {
                        int i = 0;
                        foreach (ArgSampleAttribute sample in optArg.Samples)
                        {
                            if (sample.Lines.Count() > 0)
                            {
                                i++;
                                w.WriteLine(string.Format(indentLv2 + Properties.Resources.Prog_Usage_SampleTitle, i));
                                foreach (string line in sample.Lines)
                                {
                                    w.WriteLine(indentLv2 + line);
                                }
                                w.WriteLine();
                            }
                        }
                    }
                }
            }
        }
    }

    class ArgInfo
    {
        public ArgInfo(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; private set; }
        public ArgDescriptionAttribute Description { get; set; }
        public IEnumerable<ArgSampleAttribute> Samples { get; set; }
    }

    class OptArgInfo : ArgInfo
    {
        public OptArgInfo(PropertyInfo propertyInfo, OptionalArgAttribute arg) : base(propertyInfo)
        {
            Argument = arg;
        }

        public OptionalArgAttribute Argument { get; private set; }
        public OptionalArgValueAttribute Value { get; set; }
    }

    class PosArgInfo : ArgInfo
    {
        public PosArgInfo(PropertyInfo propertyInfo, PositionalArgAttribute arg) : base(propertyInfo)
        {
            Argument = arg;
        }

        public PositionalArgAttribute Argument { get; set; }
    }

}
