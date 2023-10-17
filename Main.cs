using Godot;
using System;

public partial class Main : Node2D
{
  private Image img;
	public override void _Ready()
	{
    this.GetNode<Button>("VBoxContainer/ImportBtn").Connect("pressed", Callable.From(() => {
      this.GetNode<FileDialog>("FileDialog").PopupCentered();
    }));

    this.GetNode<FileDialog>("FileDialog").Connect("file_selected", Callable.From<string>(path => {
      this.img = Image.LoadFromFile(path);
      this.GetNode<Label>("VBoxContainer/PngInfo").Text = imgInfo(img);
    }));
	}

  private static string imgInfo(Image img) {
    return $"PNG image info: Width={img.GetWidth()}px, Height={img.GetHeight()}px.";
  }
}
