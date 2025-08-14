namespace UndertaleModTool.ProjectTool.Resources
{
	public static class Constants
	{
		public static int SLEEP_MARGIN;
		public static uint DRAW_COLOUR;

		public static void Init()
		{
			SLEEP_MARGIN = 10;
			DRAW_COLOUR = 4294967295;

			foreach (var source in Dump.Data.Options.Constants)
			{
				if (source.Name.Content == "@@SleepMargin")
				{
					if (int.TryParse(source.Value.Content, out int result))
						SLEEP_MARGIN = result;
					continue;
				}
				if (source.Name.Content == "@@DrawColour")
				{
					if (uint.TryParse(source.Value.Content, out uint result))
						DRAW_COLOUR = result;
					continue;
				}

				// TODO: macros that replace built-in functions... for some reason.
				// ^ Name: "draw_text" Value: "\n#line gml_Script_SCRIPT LINE\nVALUE"
			}
		}
	}
}
