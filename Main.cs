using Godot;
using System;

public partial class Main : Node2D
{
  private Image img;
  private int spriteWidth;
	public override void _Ready()
	{
    this.GetNode<Button>("VBoxContainer/ImportBtn").Connect("pressed", Callable.From(() => {
      this.GetNode<FileDialog>("OpenFileDialog").PopupCentered();
    }));

    this.GetNode<FileDialog>("OpenFileDialog").Connect("file_selected", Callable.From<string>(path => {
      this.img = Image.LoadFromFile(path);
      this.GetNode<Label>("VBoxContainer/PngInfo").Text = imgInfo(img);
      this.GetNode<Button>("VBoxContainer/ExportBtn").Disabled = false;
    }));

    this.GetNode<SpinBox>("VBoxContainer/HSplitContainer/WidthInputBox").Connect("value_changed", Callable.From<float>(num => {
      this.spriteWidth = (int) num;
    }));

    this.GetNode<Button>("VBoxContainer/ExportBtn").Connect("pressed", Callable.From(() => {
      this.GetNode<FileDialog>("SaveFileDialog").PopupCentered();
    }));

    this.GetNode<FileDialog>("SaveFileDialog").Connect("file_selected", Callable.From<string>(path => {
      this.img.SavePng(path);
      this.GetNode<Label>("VBoxContainer/ExportInfo").Text = $"Exported to: {path}";
    }));
	}

  private static string imgInfo(Image img) {
    return $"PNG image info: Width={img.GetWidth()}px, Height={img.GetHeight()}px.";
  }
}
