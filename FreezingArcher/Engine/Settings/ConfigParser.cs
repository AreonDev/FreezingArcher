using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FreezingArcher.Settings.Interfaces;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Configuration parser.
    /// </summary>
    public static class ConfigParser
    {
        private static string loggerModuleName = "ConfigParser";
        private static char[] trimChars = new char[] {' ', '\t'};
        private static IGroup currentGroup = null;

        /// <summary>
        /// Parses a configuration from a file.
        /// </summary>
        /// <param name="url">The path to the configuration file.</param>
        /// <param name="settings">The settings instance to parse it to.</param>
        public static void ParseFromFile (string url, ISettings settings)
        {
            string line;

            if (!File.Exists (url))
            {
                //Logger.Log.addLogEntry (LogLevel.Error, loggerModuleName,
                //                        "The config file \"{0}\" does not exist!", url);FIXME
                return;
            }

            // Read the file and display it line by line.
            StreamReader file = new StreamReader (url);
            IComment commentstorage = settings;
            for (int counter = 1; (line = file.ReadLine()) != null; counter++)
            {
                int invalid = line.Count (c => {
                    if ((c < 33 || c > 126) && c != '\n' && c != '\t' && c != '\r' && c != ' ')
                    {
                        //Logger.Log.addLogEntry (LogLevel.Error, loggerModuleName,
                        //                        "The character '{0}' in line {1} is invalid!",
                        //                        (int) c, counter);FIXME
                        return true;
                    }
                    return false;
                });

                if (invalid > 0)
                    return;

                line = line.Trim (trimChars);
                if (line.Length <= 0)
                {
                    commentstorage.Comments.Add (line);
                    continue;
                }

                switch (line[0])
                {
                case '#':
                    commentstorage.Comments.Add (line);
                    break;
                case '[':
                    // new group begins
                    int groupend = line.IndexOf (']');
                    string groupName = line.Substring (1, groupend - 1);
                    string invgroup = line.Substring (groupend + 1);
                    if (invgroup.Length > 0)
                    {
                        //Logger.Log.addLogEntry (LogLevel.Error, loggerModuleName,
                        //                        "Invalid token '{0}' in line {1}", invgroup, counter);FIXME
                    }
                    currentGroup = new Group (groupName);
                    commentstorage = currentGroup;
                    settings.Groups.Add (currentGroup);
                    break;
                default:
                    // property declaration
                    if (currentGroup == null)
                    {
                        //Logger.Log.addLogEntry (LogLevel.Error, loggerModuleName,
                        //                        "Properties must be assigned to a goup!");FIXME
                        return;
                    }
                    IProperty prop = ParseProperty (line, counter);
                    if (prop != null)
                    {
                        currentGroup.Properties.Add (prop);
                        commentstorage = prop;
                    }
                    break;
                }
            }

            file.Close();

            return;
        }

        private static IProperty ParseProperty (string line, int linenum)
        {
            int split = line.IndexOf ('=');
            string var = line.Substring (0, split).Trim (trimChars);
            string val = line.Substring (split + 1).Trim (trimChars);

            int invalid = var.Count (c => {
                if (!((c >= 64 && c <= 90) || (c >= 97 && c <= 122) || c != '+' || c != '-' || c != '_'))
                {
                    //Logger.Log.addLogEntry (LogLevel.Error, loggerModuleName,
                    //                        "The character '{0}' in variable '{1}' on line {2} is invalid!",
                    //                        c, var, linenum);FIXME
                    return true;
                }
                return false;
            });

            if (invalid > 0)
                return null;

            IProperty prop = new Property (var);

            int commas = val.Count (c => c == ',');
            if (commas > 0)
            {
                int commentidx = val.IndexOf ('#');
                string comment = "";
                if (commentidx >= 0)
                {
                    comment += val.Substring (commentidx);
                    val = val.Substring (0, commentidx);
                }

                List<string> matches = new List<string> ();
                Match match = Regex.Match (val, @"([\w\. +\-:;\(\)\[\]\{\}\$]+)(,[ \t]*|$)");
                while (match.Success)
                {
                    matches.Add (match.Groups[1].Value);
                    match = match.NextMatch ();
                }
                List<IValue> vals = new List<IValue> ();
                matches.ForEach (s => vals.Add (ParseValue (s)));
                prop.Value = new ArrayValue (vals);
                prop.LineComment = comment;
            }
            else
            {
                // parse value
                int commentidx = val.IndexOf ('#');
                string comment = "";
                if (commentidx >= 0)
                {
                    comment += val.Substring (commentidx);
                    val = val.Substring (0, commentidx);
                }

                prop.LineComment = comment;
                prop.Value = ParseValue (val);
            }

            return prop;
        }

        private static IValue ParseValue (string val)
        {
            if (val[0] == '$')
            {
                string v = val.Substring (1);
                IProperty prop = currentGroup[v];
                if (prop == null)
                {
                    //Logger.Log.addLogEntry (LogLevel.Error, loggerModuleName,
                    //                        "The referenced variable \"{0}\" is not defined in the current group!",
                    //                        val);
                    return new StringValue (val);
                }
                return new ReferencedValue (v, prop.Value.Clone (), currentGroup);
            }
            else
            {
                int i;
                bool isInt = int.TryParse (val, out i);
                if (!isInt)
                {
                    double d;
                    bool isFloat = double.TryParse (val, NumberStyles.Float,
                                                    CultureInfo.InvariantCulture.NumberFormat, out d);
                    if (!isFloat)
                        return new StringValue (val);
                    else
                        return new FloatValue (d);
                }
                else
                    return new IntegerValue (i);
            }
        }
    }
}
