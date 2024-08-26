using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace tfmStandalone
{
	public sealed class GameSettings
	{
        private static readonly string SettingsFile = AppDomain.CurrentDomain.BaseDirectory + "Settings.xml";
        public EventHandler SettingsSaved;
        public FlashPlayer.AlignmentModeEnum AlignmentMode { get; set; }
		public FlashPlayer.ZoomModeEnum ZoomMode { get; set; }
		public FlashPlayer.QualityEnum Quality { get; set; }
		public string HomeRoom { get; set; }
		public int GifLength { get; set; }
		public string BadNameReason { get; set; }
		public Theme Theme { get; set; }
		public bool UseCustomConnectionLogWindow { get; set; }
		public bool UseCustomCasierWindow { get; set; }
		public bool FilterModoChat { get; set; }
		public bool AlertModoChat { get; set; }
		public bool LogModoChat { get; set; }
		public bool FilterArbChat { get; set; }
		public bool AlertArbChat { get; set; }
		public bool LogArbChat { get; set; }
		public bool FilterServeurMessages { get; set; }
		public bool AlertServeurMessages { get; set; }
		public bool LogServeurMessages { get; set; }
		public bool FilterMapCrewChat { get; set; }
		public bool AlertMapCrewChat { get; set; }
		public bool LogMapCrewChat { get; set; }
		public bool FilterLuaTeamChat { get; set; }
		public bool AlertLuaTeamChat { get; set; }
		public bool LogLuaTeamChat { get; set; }
		public bool FilterFunCorpChat { get; set; }
		public bool AlertFunCorpChat { get; set; }
		public bool LogFunCorpChat { get; set; }
		public bool FilterFashionSquadChat { get; set; }
		public bool AlertFashionSquadChat { get; set; }
		public bool LogFashionSquadChat { get; set; }
		public bool FilterTribeChat { get; set; }
		public bool AlertTribeChat { get; set; }
		public bool LogTribeChat { get; set; }
		public bool FilterWhispers { get; set; }
		public bool AlertWhispers { get; set; }
		public bool LogWhispers { get; set; }
		public FontFamily FontFamily { get; set; }
		public int FontSize { get; set; }

		public GameSettings()
		{
			this.AlignmentMode = FlashPlayer.AlignmentModeEnum.Center;
			this.ZoomMode = FlashPlayer.ZoomModeEnum.NoZoom;
			this.Quality = FlashPlayer.QualityEnum.Medium;
			this.HomeRoom = "*a bootcamp";
			this.GifLength = 10;
			this.BadNameReason = "Name violation, please create a new account. Don't log in on this account again or you'll receive a 360h ban.";
			this.Theme = Theme.Dark;
			this.UseCustomConnectionLogWindow = true;
			this.UseCustomCasierWindow = true;
			this.FilterModoChat = true;
			this.AlertModoChat = true;
			this.LogModoChat = true;
			this.FilterArbChat = true;
			this.AlertArbChat = true;
			this.LogArbChat = true;
			this.FilterServeurMessages = true;
			this.AlertServeurMessages = false;
			this.LogServeurMessages = false;
			this.FilterMapCrewChat = true;
			this.AlertMapCrewChat = true;
			this.LogMapCrewChat = true;
			this.FilterLuaTeamChat = true;
			this.AlertLuaTeamChat = true;
			this.LogLuaTeamChat = true;
			this.FilterFunCorpChat = true;
			this.AlertFunCorpChat = true;
			this.LogFunCorpChat = true;
			this.FilterFashionSquadChat = true;
			this.AlertFashionSquadChat = true;
			this.LogFashionSquadChat = true;
			this.FilterTribeChat = true;
			this.AlertTribeChat = true;
			this.LogTribeChat = true;
			this.FilterWhispers = true;
			this.AlertWhispers = true;
			this.LogWhispers = true;
			this.FontFamily = Fonts.SystemFontFamilies.FirstOrDefault((FontFamily f) => f.Source == "Verdana");
			this.FontSize = 13;
			this.Load();
		}

		private void Load()
		{
			if (File.Exists(GameSettings.SettingsFile))
			{
				try
				{
					XElement xelement = XDocument.Load(GameSettings.SettingsFile).Element("Settings");
					XElement xelement2 = (xelement != null) ? xelement.Element("FlashSettings") : null;
					if (xelement2 != null)
					{
						this.AlignmentMode = (FlashPlayer.AlignmentModeEnum)((int)xelement2.Element("AlignmentMode"));
						this.ZoomMode = (FlashPlayer.ZoomModeEnum)((int)xelement2.Element("ZoomMode"));
						this.Quality = (FlashPlayer.QualityEnum)((int)xelement2.Element("Quality"));
					}
					this.HomeRoom = (((string)((xelement != null) ? xelement.Element("HomeRoom") : null)) ?? "*a bootcamp");
					XElement xelement3 = (xelement != null) ? xelement.Element("GifLength") : null;
					this.GifLength = ((xelement3 == null) ? 10 : ((int)xelement3));
					if (this.GifLength < 5)
					{
						this.GifLength = 5;
					}
					if (this.GifLength > 60)
					{
						this.GifLength = 60;
					}
					string text = (string)((xelement != null) ? xelement.Element("BadNameReason") : null);
					if (!string.IsNullOrWhiteSpace(text))
					{
						this.BadNameReason = text;
					}
					this.Theme = (Theme)((int)((xelement != null) ? xelement.Element("Theme") : null));
					XElement xelement4 = (xelement != null) ? xelement.Element("UseCustomConnectionLogWindow") : null;
					if (xelement4 != null)
					{
						this.UseCustomConnectionLogWindow = (bool)xelement4;
					}
					XElement xelement5 = (xelement != null) ? xelement.Element("UseCustomCasierWindow") : null;
					if (xelement5 != null)
					{
						this.UseCustomCasierWindow = (bool)xelement5;
					}
					XElement xelement6 = (xelement != null) ? xelement.Element("ChatSettings") : null;
					if (xelement6 != null)
					{
						XElement xelement7 = xelement6.Element("FilterModoChat");
						if (xelement7 != null)
						{
							this.FilterModoChat = (bool)xelement7;
						}
						XElement xelement8 = xelement6.Element("AlertModoChat");
						if (xelement8 != null)
						{
							this.AlertModoChat = (bool)xelement8;
						}
						XElement xelement9 = xelement6.Element("LogModoChat");
						if (xelement9 != null)
						{
							this.LogModoChat = (bool)xelement9;
						}
						XElement xelement10 = xelement6.Element("FilterArbChat");
						if (xelement10 != null)
						{
							this.FilterArbChat = (bool)xelement10;
						}
						XElement xelement11 = xelement6.Element("AlertArbChat");
						if (xelement11 != null)
						{
							this.AlertArbChat = (bool)xelement11;
						}
						XElement xelement12 = xelement6.Element("LogArbChat");
						if (xelement12 != null)
						{
							this.LogArbChat = (bool)xelement12;
						}
						XElement xelement13 = xelement6.Element("FilterServeurMessages");
						if (xelement13 != null)
						{
							this.FilterServeurMessages = (bool)xelement13;
						}
						XElement xelement14 = xelement6.Element("AlertServeurMessages");
						if (xelement14 != null)
						{
							this.AlertServeurMessages = (bool)xelement14;
						}
						XElement xelement15 = xelement6.Element("LogServeurMessages");
						if (xelement15 != null)
						{
							this.LogServeurMessages = (bool)xelement15;
						}
						XElement xelement16 = xelement6.Element("FilterMapCrewChat");
						if (xelement16 != null)
						{
							this.FilterMapCrewChat = (bool)xelement16;
						}
						XElement xelement17 = xelement6.Element("AlertMapCrewChat");
						if (xelement17 != null)
						{
							this.AlertMapCrewChat = (bool)xelement17;
						}
						XElement xelement18 = xelement6.Element("LogMapCrewChat");
						if (xelement18 != null)
						{
							this.LogMapCrewChat = (bool)xelement18;
						}
						XElement xelement19 = xelement6.Element("FilterLuaTeamChat");
						if (xelement19 != null)
						{
							this.FilterLuaTeamChat = (bool)xelement19;
						}
						XElement xelement20 = xelement6.Element("AlertLuaTeamChat");
						if (xelement20 != null)
						{
							this.AlertLuaTeamChat = (bool)xelement20;
						}
						XElement xelement21 = xelement6.Element("LogLuaTeamChat");
						if (xelement21 != null)
						{
							this.LogLuaTeamChat = (bool)xelement21;
						}
						XElement xelement22 = xelement6.Element("FilterFunCorpChat");
						if (xelement22 != null)
						{
							this.FilterFunCorpChat = (bool)xelement22;
						}
						XElement xelement23 = xelement6.Element("AlertFunCorpChat");
						if (xelement23 != null)
						{
							this.AlertFunCorpChat = (bool)xelement23;
						}
						XElement xelement24 = xelement6.Element("LogFunCorpChat");
						if (xelement24 != null)
						{
							this.LogFunCorpChat = (bool)xelement24;
						}
						XElement xelement25 = xelement6.Element("FilterFashionSquadChat");
						if (xelement25 != null)
						{
							this.FilterFashionSquadChat = (bool)xelement25;
						}
						XElement xelement26 = xelement6.Element("AlertFashionSquadChat");
						if (xelement26 != null)
						{
							this.AlertFashionSquadChat = (bool)xelement26;
						}
						XElement xelement27 = xelement6.Element("LogFashionSquadChat");
						if (xelement27 != null)
						{
							this.LogFashionSquadChat = (bool)xelement27;
						}
						XElement xelement28 = xelement6.Element("FilterTribeChat");
						if (xelement28 != null)
						{
							this.FilterTribeChat = (bool)xelement28;
						}
						XElement xelement29 = xelement6.Element("AlertTribeChat");
						if (xelement29 != null)
						{
							this.AlertTribeChat = (bool)xelement29;
						}
						XElement xelement30 = xelement6.Element("LogTribeChat");
						if (xelement30 != null)
						{
							this.LogTribeChat = (bool)xelement30;
						}
						XElement xelement31 = xelement6.Element("FilterWhispers");
						if (xelement31 != null)
						{
							this.FilterWhispers = (bool)xelement31;
						}
						XElement xelement32 = xelement6.Element("AlertWhispers");
						if (xelement32 != null)
						{
							this.AlertWhispers = (bool)xelement32;
						}
						XElement xelement33 = xelement6.Element("LogWhispers");
						if (xelement33 != null)
						{
							this.LogWhispers = (bool)xelement33;
						}
						XElement fontFamily = xelement6.Element("FontFamily");
						if (fontFamily != null)
						{
							this.FontFamily = Fonts.SystemFontFamilies.FirstOrDefault((FontFamily f) => f.Source == (string)fontFamily);
						}
						XElement xelement34 = xelement6.Element("FontSize");
						if (xelement34 != null)
						{
							this.FontSize = (int)xelement34;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			((App)Application.Current).SetTheme(this.Theme);
		}

		public void Save()
		{
			new XDocument(new object[]
			{
				new XElement("Settings", new object[]
				{
					new XElement("FlashSettings", new object[]
					{
						new XElement("AlignmentMode", (int)this.AlignmentMode),
						new XElement("ZoomMode", (int)this.ZoomMode),
						new XElement("Quality", (int)this.Quality)
					}),
					new XElement("HomeRoom", this.HomeRoom),
					new XElement("GifLength", this.GifLength),
					new XElement("BadNameReason", this.BadNameReason),
					new XElement("Theme", (int)this.Theme),
					new XElement("UseCustomConnectionLogWindow", this.UseCustomConnectionLogWindow),
					new XElement("UseCustomCasierWindow", this.UseCustomCasierWindow),
					new XElement("ChatSettings", new object[]
					{
						new XElement("FilterModoChat", this.FilterModoChat),
						new XElement("AlertModoChat", this.AlertModoChat),
						new XElement("LogModoChat", this.LogModoChat),
						new XElement("FilterArbChat", this.FilterArbChat),
						new XElement("AlertArbChat", this.AlertArbChat),
						new XElement("LogArbChat", this.LogArbChat),
						new XElement("FilterServeurMessages", this.FilterServeurMessages),
						new XElement("AlertServeurMessages", this.AlertServeurMessages),
						new XElement("LogServeurMessages", this.LogServeurMessages),
						new XElement("FilterMapCrewChat", this.FilterMapCrewChat),
						new XElement("AlertMapCrewChat", this.AlertMapCrewChat),
						new XElement("LogMapCrewChat", this.LogMapCrewChat),
						new XElement("FilterLuaTeamChat", this.FilterLuaTeamChat),
						new XElement("AlertLuaTeamChat", this.AlertLuaTeamChat),
						new XElement("LogLuaTeamChat", this.LogLuaTeamChat),
						new XElement("FilterFunCorpChat", this.FilterFunCorpChat),
						new XElement("AlertFunCorpChat", this.AlertFunCorpChat),
						new XElement("LogFunCorpChat", this.LogFunCorpChat),
						new XElement("FilterFashionSquadChat", this.FilterFashionSquadChat),
						new XElement("AlertFashionSquadChat", this.AlertFashionSquadChat),
						new XElement("LogFashionSquadChat", this.LogFashionSquadChat),
						new XElement("FilterTribeChat", this.FilterTribeChat),
						new XElement("AlertTribeChat", this.AlertTribeChat),
						new XElement("LogTribeChat", this.LogTribeChat),
						new XElement("FilterWhispers", this.FilterWhispers),
						new XElement("AlertWhispers", this.AlertWhispers),
						new XElement("LogWhispers", this.LogWhispers),
						new XElement("FontFamily", this.FontFamily.Source),
						new XElement("FontSize", this.FontSize)
					})
				})
			}).Save(GameSettings.SettingsFile);
			TaskHelpers.UiInvoke(delegate
			{
				((App)Application.Current).SetTheme(this.Theme);
				EventHandler settingsSaved = this.SettingsSaved;
				if (settingsSaved == null)
				{
					return;
				}
				settingsSaved(this, EventArgs.Empty);
			});
		}
	}
}
