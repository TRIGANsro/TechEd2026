using System.Windows;

namespace WPF2DBinding.Models;

public class ZajimavostDto
{
    public Guid Id { get; set; }
    public string Nadpis { get; set; } = string.Empty;
    public string Popis { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public static ZajimavostDto FromZajimavost(Zajimavost z) => new()
    {
        Id = z.Id,
        Nadpis = z.Nadpis,
        Popis = z.Popis,
        X = z.Oblast.X,
        Y = z.Oblast.Y,
        Width = z.Oblast.Width,
        Height = z.Oblast.Height
    };

    public Zajimavost ToZajimavost() => new()
    {
        Id = Id,
        Nadpis = Nadpis,
        Popis = Popis,
        Oblast = new Rect(X, Y, Width, Height)
    };
}