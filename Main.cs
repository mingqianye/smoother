using Godot;
using System;

public partial class Main : PanelContainer
{
  private Image img;
  private int spriteWidth, factor;
	public override void _Ready()
	{
    this.GetNode<Button>("VBoxContainer/ImportBtn").Connect("pressed", Callable.From(() => {
      this.GetNode<FileDialog>("OpenFileDialog").PopupCentered();
    }));

    this.GetNode<FileDialog>("OpenFileDialog").Connect("file_selected", Callable.From<string>(path => {
      this.img = Image.LoadFromFile(path);
      this.GetNode<Label>("VBoxContainer/PngInfo").Text = $"Imported PNG:\n{imgInfo(img)}";

      SpinBox sb = this.GetNode<SpinBox>("VBoxContainer/HSplitContainer/WidthInputBox");
      sb.MinValue = 1;
      sb.MaxValue = img.GetWidth() / 2;

      this.GetNode<Button>("VBoxContainer/ExportBtn").Disabled = false;
      this.GetNode<FileDialog>("SaveFileDialog").CurrentDir = this.GetNode<FileDialog>("OpenFileDialog").CurrentDir;
      this.GetNode<FileDialog>("SaveFileDialog").CurrentFile = "output.png";
    }));

    this.GetNode<SpinBox>("VBoxContainer/HSplitContainer/WidthInputBox").Connect("value_changed", Callable.From<float>(num => {
      this.spriteWidth = (int) num;
    }));

    this.GetNode<SpinBox>("VBoxContainer/HSplitContainer2/FactorInputBox").Connect("value_changed", Callable.From<float>(num => {
      this.factor = (int) num;
    }));

    this.GetNode<Button>("VBoxContainer/ExportBtn").Connect("pressed", Callable.From(() => {
      this.GetNode<FileDialog>("SaveFileDialog").PopupCentered();
    }));

    this.GetNode<FileDialog>("SaveFileDialog").Connect("file_selected", Callable.From<string>(path => {
      Image newImage = ImageComputer.Interpolate(this.img, this.spriteWidth, this.factor);
      newImage.SavePng(path);
      this.GetNode<Label>("VBoxContainer/ExportInfo").Text = $"Exported to: {path}. {imgInfo(newImage)}";
    }));
	}

  private static string imgInfo(Image img) {
    return $"PNG image info: Width={img.GetWidth()}px, Height={img.GetHeight()}px.";
  }
}
