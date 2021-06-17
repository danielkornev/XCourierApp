using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCourierApp
{
	public static class Constants
	{
		public static readonly string OFFLINE_DATABASE_NAME = "JournalAppStore.litedb";

        public static class Colors
        {
            public static string Violet = "#FF93207D";
            public static string Orange = "#FFD4B21C";
            public static string LightGreen = "#FF8CAC1D";
            public static string LightBlue = "#FF1C8FC6";
            public static string KingBrown = "#FF862526";
            public static string DarkGray = "#FF414141";
            public static string BeautifulGreen = "#FF1DA07E";
            public static string MidGray = "#FF646464";
        }

        public enum Side
        {
            Left,
            Right,
            Unknown
        }

        public static class KnownEntities
        {
            /// <summary>
            /// Used for the First Journal.
            /// </summary>
            public static Guid FirstJournal = Guid.Parse("0E4A20E5-5572-4C69-A2F0-9FE25276F1C5");
        }

        public static class Settings
        {
            public static class Defaults
            {
                public static string DefaultLanguage = "en-US";
            }
        }

        public static class Search
        {
            public static class Properties
            {
                public static string ParentJournalId = "ParentJournalId";
                public static string Kind = "StorageItemKind";
                public static string PageId = "PageId";
            }

            public static string IndexName = "CourierShellIdx";
        }

        public static class Ontology
        {
            public static class Kinds
            {
                public static string Page = "CourierShell.Kinds.Page";
            }
        }

        public static string LanguageTagToName(string languageTag)
        {
            var enCA = "Microsoft English (Canada) Handwriting Recognizer";
            var enUS = "Microsoft English (US) Handwriting Recognizer";
            var ru = "Microsoft система распознавания русского рукописного ввода";

            Dictionary<string, string> KnownLanguages = new Dictionary<string, string>();

            KnownLanguages.Add("en-CA", enCA);
            KnownLanguages.Add("en-US", enUS);
            KnownLanguages.Add("ru", ru);

            if (KnownLanguages.Keys.Contains(languageTag))
                return KnownLanguages[languageTag];

            return string.Empty;
        }
    } // class
} // namespace
