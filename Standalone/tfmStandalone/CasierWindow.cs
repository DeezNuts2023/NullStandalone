using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace tfmStandalone
{
    /// <summary>
    /// Interaction logic for CasierWindow.xaml
    /// </summary>
    public partial class CasierWindow : PinnableWindow
    {
        public CasierWindow(string text)
        {
            this.InitializeComponent();
            base.DataContext = this;
            List<string> list = text.Replace("<font size='8'>\n</font>", "\n").Split(new string[]
            {
                "\n- "
            }, StringSplitOptions.None).ToList<string>();
            string value = Regex.Match(list[0], "<p align='center'>Sanction logs for <v>(?<key>.+)</v></p>(?:.*)").Groups["key"].Value;
            base.Title = string.Format("Casier ({0})", value);
            if (list.Count > 1)
            {
                int num = 0;
                if (list[0].Contains("Currently running sanctions"))
                {
                    num = list.IndexOf(list.Last((string l) => l.EndsWith("\n\n\n")));
                }
                FlowDocument flowDocument = new FlowDocument
                {
                    FontFamily = new FontFamily("Verdana"),
                    PagePadding = new Thickness(10.0)
                };
                Paragraph paragraph = new Paragraph
                {
                    FontSize = 12.0,
                    Margin = new Thickness(0.0),
                    TextAlignment = TextAlignment.Center
                };
                paragraph.Inlines.Add(this.CreateRun("Tfm_N", "Sanction logs for ", 12.0, false));
                paragraph.Inlines.Add(this.CreateRun("Tfm_V", value, 12.0, false));
                flowDocument.Blocks.Add(paragraph);
                flowDocument.Blocks.Add(new Paragraph
                {
                    Margin = new Thickness(0.0),
                    FontSize = 12.0
                });
                if (num > 0)
                {
                    Paragraph paragraph2 = new Paragraph
                    {
                        FontSize = 12.0,
                        Margin = new Thickness(0.0)
                    };
                    paragraph2.Inlines.Add(new Run
                    {
                        Foreground = this.GetColor("#FFC2C2DA"),
                        Text = "Currently running sanctions:"
                    });
                    flowDocument.Blocks.Add(paragraph2);
                    flowDocument.Blocks.Add(new Paragraph
                    {
                        Margin = new Thickness(0.0),
                        FontSize = 8.0
                    });
                }
                for (int i = 1; i < list.Count; i++)
                {
                    CasierItem casierItem = this.ProcessItem(list[i], num > 0 && i <= num);
                    if (casierItem != null)
                    {
                        if (casierItem.ItemType == CasierItem.Type.Ban || casierItem.ItemType == CasierItem.Type.Mute)
                        {
                            SanctionCasierItem sanctionCasierItem = (SanctionCasierItem)casierItem;
                            Paragraph paragraph3 = new Paragraph
                            {
                                Margin = new Thickness(0.0)
                            };
                            paragraph3.Inlines.Add(this.CreateRun("Tfm_N", "- ", 12.0, false));
                            paragraph3.Inlines.Add(this.CreateRun(sanctionCasierItem.IsCancelled ? "Tfm_G" : "Tfm_V", (sanctionCasierItem.ItemType == CasierItem.Type.Ban) ? string.Format("BAN {0}", sanctionCasierItem.Hours) : string.Format("MUTE {0}", sanctionCasierItem.Hours), 12.0, true));
                            paragraph3.Inlines.Add(this.CreateRun(sanctionCasierItem.IsCancelled ? "Tfm_G" : "Tfm_N", string.Format(" ({0}) by {1} : ", sanctionCasierItem.Target, sanctionCasierItem.Author), 11.0, false));
                            paragraph3.Inlines.Add(this.CreateRun(sanctionCasierItem.IsCancelled ? "Tfm_G" : "Tfm_BL", sanctionCasierItem.Reason, 11.0, false));
                            flowDocument.Blocks.Add(paragraph3);
                            if (sanctionCasierItem.IsCancelled)
                            {
                                Paragraph paragraph4 = new Paragraph
                                {
                                    Margin = new Thickness(0.0, 2.0, 0.0, 0.0)
                                };
                                paragraph4.Inlines.Add(new Run
                                {
                                    FontSize = 12.0,
                                    Text = "  "
                                });
                                paragraph4.Inlines.Add(this.CreateRun("Tfm_G", sanctionCasierItem.IsOverriden ? "Overriden" : string.Format("Cancelled by {0}", sanctionCasierItem.CancellationAuthor), 9.0, false));
                                if (!sanctionCasierItem.IsOverriden && !string.IsNullOrWhiteSpace(sanctionCasierItem.CancellationReason))
                                {
                                    paragraph4.Inlines.Add(this.CreateRun("Tfm_G", sanctionCasierItem.CancellationReason, 9.0, false));
                                }
                                flowDocument.Blocks.Add(paragraph4);
                            }
                            Paragraph paragraph5 = new Paragraph
                            {
                                Margin = new Thickness(0.0, 2.0, 0.0, 0.0)
                            };
                            paragraph5.Inlines.Add(new Run
                            {
                                FontSize = 12.0,
                                Text = "  "
                            });
                            paragraph5.Inlines.Add(this.CreateRun(sanctionCasierItem.IsCancelled ? "Tfm_G" : "Tfm_N2", this.GetTimeString(sanctionCasierItem.GivenTime, sanctionCasierItem.StartTime, sanctionCasierItem.EndTime, sanctionCasierItem.CancellationTime), 9.0, false));
                            flowDocument.Blocks.Add(paragraph5);
                        }
                        else
                        {
                            NameChangeCasierItem nameChangeCasierItem = (NameChangeCasierItem)casierItem;
                            Paragraph paragraph6 = new Paragraph
                            {
                                Margin = new Thickness(0.0)
                            };
                            paragraph6.Inlines.Add(this.CreateRun("Tfm_N", "- ", 12.0, false));
                            paragraph6.Inlines.Add(this.CreateRun("Tfm_V", "NAME CHANGE", 12.0, true));
                            paragraph6.Inlines.Add(this.CreateRun("Tfm_N", " : ", 11.0, false));
                            paragraph6.Inlines.Add(this.CreateRun("Tfm_BV", nameChangeCasierItem.OldName, 11.0, false));
                            paragraph6.Inlines.Add(this.CreateRun("Tfm_N", " → ", 11.0, false));
                            paragraph6.Inlines.Add(this.CreateRun("Tfm_BV", nameChangeCasierItem.NewName, 11.0, false));
                            flowDocument.Blocks.Add(paragraph6);
                            Paragraph paragraph7 = new Paragraph
                            {
                                Margin = new Thickness(0.0, 2.0, 0.0, 0.0)
                            };
                            paragraph7.Inlines.Add(new Run
                            {
                                FontSize = 12.0,
                                Text = "  "
                            });
                            paragraph7.Inlines.Add(this.CreateRun("Tfm_N2", nameChangeCasierItem.Time, 9.0, false));
                            flowDocument.Blocks.Add(paragraph7);
                        }
                        flowDocument.Blocks.Add(new Paragraph
                        {
                            Margin = new Thickness(0.0),
                            FontSize = 8.0
                        });
                    }
                    if (i == num)
                    {
                        flowDocument.Blocks.Add(new Paragraph
                        {
                            Margin = new Thickness(0.0),
                            FontSize = 8.0
                        });
                        flowDocument.Blocks.Add(new Paragraph
                        {
                            Margin = new Thickness(0.0),
                            FontSize = 8.0
                        });
                    }
                }
                this.FlowDocumentScrollViewer.Document = flowDocument;
            }
        }

        private SolidColorBrush GetColor(string hexString)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFrom(hexString);
        }

        private Run CreateRun(string foregroundResourceName, string text, double fontSize = 12.0, bool isBold = false)
        {
            Run run = new Run
            {
                Text = text,
                FontSize = fontSize
            };
            if (isBold)
            {
                run.FontWeight = FontWeights.Bold;
            }
            run.SetResourceReference(TextElement.ForegroundProperty, foregroundResourceName);
            return run;
        }

        private string GetTimeString(string givenTime, string startTime, string endTime, string cancellationTime)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrWhiteSpace(givenTime))
            {
                list.Add(givenTime);
            }
            if (!string.IsNullOrWhiteSpace(startTime) && !string.IsNullOrWhiteSpace(endTime))
            {
                list.Add(string.Format("{0} → {1}", startTime, endTime));
            }
            if (!string.IsNullOrWhiteSpace(cancellationTime))
            {
                list.Add(cancellationTime);
            }
            return string.Join("  |  ", list);
        }

        private CasierItem ProcessItem(string item, bool isCurrent)
        {
            string[] array = item.Trim().Trim(new char[]
            {
                '\n'
            }).Split(new char[]
            {
                '\n'
            });
            bool flag = array[0].StartsWith("<g>");
            array[0] = array[0].Substring(3);
            if (flag)
            {
                array[array.Length - 1] = array[array.Length - 1].Substring(0, array[array.Length - 1].Length - 4);
            }
            string a = string.Empty;
            if (array[0].StartsWith("<b>MUTE"))
            {
                a = "MUTE";
            }
            else if (array[0].StartsWith("<b>BAN"))
            {
                a = "BAN";
            }
            else if (array[0].StartsWith("<b>NAME CHANGE"))
            {
                a = "NAME CHANGE";
            }
            if (a == "MUTE" || a == "BAN")
            {
                SanctionCasierItem sanctionCasierItem = new SanctionCasierItem
                {
                    ItemType = ((a == "MUTE") ? CasierItem.Type.Mute : CasierItem.Type.Ban),
                    IsCurrentlyRunning = isCurrent,
                    IsCancelled = flag
                };
                Match match = Regex.Match(array[0].Trim(), "<b>(?:BAN|MUTE) (?<hours>(.+h)|DEF)</b>(?:<n>)?<font size='11'> \\((?<target>.+)\\) by (?<author>[^\\s]+)(?: : (?:<bl>)?(?<reason>[^<]*)(?:</bl>)?)?</font>");
                sanctionCasierItem.Hours = match.Groups["hours"].Value;
                sanctionCasierItem.Target = match.Groups["target"].Value;
                sanctionCasierItem.Author = match.Groups["author"].Value;
                sanctionCasierItem.Reason = WebUtility.HtmlDecode(match.Groups["reason"].Value);
                if (sanctionCasierItem.Reason == "$MessageTriche")
                {
                    sanctionCasierItem.Reason = "Hack. Your account will be permanently banned if you keep breaking the rules!";
                }
                string input = string.Empty;
                if (flag)
                {
                    input = array[2].Trim();
                    if (array[1].Trim() == "<font size='9'>Overriden</font>")
                    {
                        sanctionCasierItem.IsOverriden = true;
                    }
                    else
                    {
                        Match match2 = Regex.Match(array[1].Trim(), "<font size='9'>Cancelled by (?<author>[^\\s]+)(?: : (?<reason>.*))?</font>");
                        sanctionCasierItem.CancellationAuthor = match2.Groups["author"].Value;
                        sanctionCasierItem.CancellationReason = WebUtility.HtmlDecode(match2.Groups["reason"].Value);
                    }
                }
                else
                {
                    input = array[1].Trim();
                }
                foreach (Regex regex in new List<Regex>
                {
                    new Regex("^<font size='9'>(?:<n2>)?(?<startDate>[^\\s]+) (?<startTime>[^\\s]+) &#8594; (?<endDate>[^\\s]+) (?<endTime>[^\\s<]+)(?:</n2>)?</font>$"),
                    new Regex("^<font size='9'>(?:<n2>)?(?<givenDate>[^\\s]+) (?<givenTime>[^\\s<]+)(?:</n2>)?</font>$"),
                    new Regex("^<font size='9'>(?:<n2>)?(?<startDate>[^\\s]+) (?<startTime>[^\\s]+) &#8594; (?<endDate>[^\\s]+) (?<endTime>[^\\s]+)  \\|  (?<cancellationDate>[^\\s]+) (?<cancellationTime>[^\\s<]+)(?:</n2>)?</font>$"),
                    new Regex("^<font size='9'>(?:<n2>)?(?<givenDate>[^\\s]+) (?<givenTime>[^\\s]+)  \\|  (?<startDate>[^\\s]+) (?<startTime>[^\\s]+) &#8594; (?<endDate>[^\\s]+) (?<endTime>[^\\s<]+)(?:</n2>)?</font>$"),
                    new Regex("^<font size='9'>(?:<n2>)?(?<givenDate>[^\\s]+) (?<givenTime>[^\\s]+)  \\|  (?<startDate>[^\\s]+) (?<startTime>[^\\s]+) &#8594; (?<endDate>[^\\s]+) (?<endTime>[^\\s]+)  \\|  (?<cancellationDate>[^\\s]+) (?<cancellationTime>[^\\s<]+)(?:</n2>)?</font>$")
                })
                {
                    Match match3 = regex.Match(input);
                    if (match3.Success)
                    {
                        sanctionCasierItem.GivenTime = string.Format("{0} {1}", match3.Groups["givenDate"], match3.Groups["givenTime"]);
                        sanctionCasierItem.StartTime = string.Format("{0} {1}", match3.Groups["startDate"], match3.Groups["startTime"]);
                        sanctionCasierItem.EndTime = string.Format("{0} {1}", match3.Groups["endDate"], match3.Groups["endTime"]);
                        sanctionCasierItem.CancellationTime = string.Format("{0} {1}", match3.Groups["cancellationDate"], match3.Groups["cancellationTime"]);
                        break;
                    }
                }
                return sanctionCasierItem;
            }
            if (!(a == "NAME CHANGE"))
            {
                return null;
            }
            NameChangeCasierItem nameChangeCasierItem = new NameChangeCasierItem();
            nameChangeCasierItem.ItemType = CasierItem.Type.NameChange;
            Match match4 = Regex.Match(array[0].Trim(), "^<b>NAME CHANGE</b></v><font size='11'> : <BV>(?<oldName>.+)</BV> &#8594; <BV>(?<newName>.+)</BV></font>$");
            nameChangeCasierItem.OldName = match4.Groups["oldName"].Value;
            nameChangeCasierItem.NewName = match4.Groups["newName"].Value;
            Match match5 = Regex.Match(array[1].Trim(), "^<font size='9'><n2>(?<date>.+)</n2></font>$");
            nameChangeCasierItem.Time = match5.Groups["date"].Value;
            return nameChangeCasierItem;
        }


    }
}
