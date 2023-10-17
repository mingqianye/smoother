using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

public static class ImageComputer {
  public static Image Interpolate(Image Image, int SpriteWidth, int Factor) {
    var newRows = toRows(Image, SpriteWidth).Map(r => interpolate(r, Factor));
    return toImage(newRows);
  }

  private static Arr<Row> toRows(Image image, int spriteWidth) {
    List<Row> rows = new List<Row>();

    for (int row = 0; row < image.GetHeight(); row++) {
      List<Block> blocks = new List<Block>();

      for (int blockId = 0; blockId < image.GetWidth() / spriteWidth; blockId++) {
        List<Color> colors = new List<Color>();

        for (int i = 0; i < spriteWidth; i++) {
          colors.Add(image.GetPixel(blockId * spriteWidth + i, row));
        }

        blocks.Add(new Block(colors.ToArr()));
      }
      rows.Add(new Row(blocks.ToArr()));
    }

    return rows.ToArr();
  }


  private static Image toImage(Arr<Row> rows) {
    Image img = Image.Create(
      width: rows[0].Blocks[0].Colors.Count * rows[0].Blocks.Count,
      height: rows.Count,
      useMipmaps: false,
      format: Image.Format.Rgba8);

    int blockWidth = rows[0].Blocks[0].Colors.Count;
    
    for (int row = 0; row < rows.Count; row++){
      for (int blockId = 0; blockId < rows[row].Blocks.Count; blockId++) {
        for (int i = 0; i < blockWidth; i++) {
          img.SetPixel(i + blockWidth * blockId, row, rows[row].Blocks[blockId].Colors[i]);
        }
      }
    }
    
    return img;
  }

  private static Row interpolate(Row row, int factor) {
    if (factor == 0) return row;
    var newBlocks = row.Blocks
      .Zip(row.Blocks.Skip(1))
      .Map(pair => interpolate(pair, factor))
      .SelectMany(x => x)
      .Append(row.Blocks.Last())
      .ToArr();
    return new Row(newBlocks);
  }

  private static Arr<Block> interpolate((Block, Block) pair, int factor) {
    Block step = (pair.Item2 - pair.Item1) / factor;

    return Enumerable.Range(0, factor + 1)
      .Map(i => pair.Item1 + step * i)
      .ToArr();
  }

  private record Row(Arr<Block> Blocks);
  private record Block(Arr<Color> Colors) {

    public static Block operator +(Block a, Block b) {
      var colors = a.Colors
        .Zip(b.Colors)
        .Map(pair => pair.Item1 + pair.Item2)
        .ToArr();
      return new Block(colors);
    }

    public static Block operator -(Block a, Block b) {
      var colors = a.Colors
        .Zip(b.Colors)
        .Map(pair => pair.Item1 - pair.Item2)
        .ToArr();
      return new Block(colors);
    }

    public static Block operator *(Block a, float i) {
      var colors = a.Colors
        .Map(c => c * i)
        .ToArr();
      return new Block(colors);
    }

    public static Block operator /(Block a, float i) {
      return a * (1/i);
    }

  };
}