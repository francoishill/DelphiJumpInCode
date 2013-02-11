using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SharedClasses;

namespace DelphiJumpInCode
{
	class Program
	{
		private enum JumpToCommands { Implementation };

		private static Predicate<string> GetLineFindPredicateForJumpToCommand(JumpToCommands jumptoCommand)
		{
			switch (jumptoCommand)
			{
				case JumpToCommands.Implementation:
					return line => line.Trim().Equals("Implementation", StringComparison.InvariantCultureIgnoreCase);
				default:
					return line => false;
			}
		}

		[STAThread]
		static int Main(string[] args)
		{
			Application.EnableVisualStyles();

			if (args.Length < 2)
				return 0;

			JumpToCommands jumptoCommand;
			if (!Enum.TryParse(args[0], true, out jumptoCommand))
			{
				UserMessages.ShowErrorMessage("Unknown JumpToCommand: " + args[0]);
				return 0;
			}

			string delphiPasFilePath = args[1];
			if (!File.Exists(delphiPasFilePath))
				return 0;

			var fileLines = File.ReadAllLines(delphiPasFilePath).ToList();
			int lineIndexWhichMatchedPredicate = fileLines.FindIndex(GetLineFindPredicateForJumpToCommand(jumptoCommand));
			if (lineIndexWhichMatchedPredicate == -1)
				return 0;

			var simulator = new WindowsInput.KeyboardSimulator();

			simulator.KeyUp(WindowsInput.Native.VirtualKeyCode.LMENU);//Because in Delphi we pressed Alt to open the Tools menu and call our "Tool"
			simulator.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_G);
			simulator.TextEntry((lineIndexWhichMatchedPredicate + 1).ToString());//Line numbers start at 1
			simulator.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);

			return 0;
		}
	}
}
