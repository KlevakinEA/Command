Text_Editor editor = new Text_Editor();
editor.IncertText("a");
Console.WriteLine(editor.DeleteText(2));
Console.WriteLine(editor.DeleteText(1));
editor.IncertText("aa");
editor.IncertText("b");
Console.WriteLine(editor.GetText());
Console.WriteLine();

Console.WriteLine(editor.Undo());
editor.Undo();
editor.Undo();
Console.WriteLine(editor.Undo());
Console.WriteLine(editor.GetText());
Console.WriteLine();

Console.WriteLine(editor.Redo());
editor.Redo();
editor.Redo();
Console.WriteLine(editor.Redo());
Console.WriteLine(editor.GetText());
Console.WriteLine();

editor.Undo();
Console.WriteLine(editor.GetText());
Console.WriteLine();

editor.DeleteText(1);
Console.WriteLine(editor.Redo());
Console.WriteLine(editor.GetText());




interface ICommand
{
    string Execute(string text);
    string Undo(string text);
}


class IncertTextCommand: ICommand
{
    private protected string change;
    public string Execute(string text) { return text + change; }
    public string Undo(string text) { return text.Substring(0, text.Length - change.Length); }
    public IncertTextCommand(string change) { this.change = change; }
}


class DeleteTextCommand : ICommand
{
    private protected string change;
    public string Execute(string text) { change = text.Substring(text.Length - change.Length, change.Length); return text.Substring(0, text.Length - change.Length); }
    public string Undo(string text) { return text + change; }
    public DeleteTextCommand(string change) { this.change = change; }
}


class Text_Editor
{
    private protected string text = "";
    private protected List<ICommand> Undo_list = new List<ICommand>();
    private protected List<ICommand> Redo_list = new List<ICommand>();
    private protected int max_undo_redo = 3;
    public string GetText() { return text; }
    public void IncertText(string change)
    {
        Redo_list.Clear();
        ICommand command = new IncertTextCommand(change);
        text = command.Execute(text);
        Undo_list.Add(command);
        if (Undo_list.Count > max_undo_redo) Undo_list.RemoveAt(0);
    }
    public bool DeleteText(int change)
    {
        if (text.Length < change) return false;
        Redo_list.Clear();
        string command_input = "";
        for (int i = 0; i < change; i++) command_input += "-";
        ICommand command = new DeleteTextCommand(command_input);
        text = command.Execute(text);
        Undo_list.Add(command);
        if (Undo_list.Count > max_undo_redo) Undo_list.RemoveAt(0);
        return true;
    }
    public bool Undo()
    {
        if(!Undo_list.Any()) return false;
        Redo_list.Add(Undo_list.Last());
        text = Undo_list.Last().Undo(text);
        Undo_list.RemoveAt(Undo_list.Count - 1);
        return true;
    }
    public bool Redo()
    {
        if(!Redo_list.Any()) return false;
        Undo_list.Add(Redo_list.Last());
        text = Redo_list.Last().Execute(text);
        Redo_list.RemoveAt(Redo_list.Count - 1);
        return true;
    }
}