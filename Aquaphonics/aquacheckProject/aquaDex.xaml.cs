using SkiaSharp;
using System.Drawing;
using Color = Microsoft.Maui.Graphics.Color;

namespace aquacheckProject;

public partial class aquaDex : ContentPage
{
	public aquaDex()
	{
		InitializeComponent();
        fborder.BackgroundColor = Color.FromRgb(51, 51, 51);
       

    }
    private void optionsPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Get the selected item from the Picker
        var selectedOption = optionsPicker.SelectedItem?.ToString();

        // Change the Image source and background color based on the selected item
        if (selectedOption == "Angelfish")
        {
            fisher.Source = "angelfish.png";
            //lightblue
            optionsPicker.FontSize = 30;
            fishRow.IsVisible = true;


            typeText.Text = "Tropical Fish, Freshwater Fish";
            lifeText.Text = "12 - 15 Years";
            tempText.Text = "24 - 29°C";
            turbidText.Text = ">5 NTU ";
            phText.Text = "6.8 - 7.8";
            tdsText.Text = "54 - 145 ppm ";

            varText.Text = "Albino, Koi, Zebra etc.";
        }
        else if (selectedOption == "Goldfish")
        {
            fisher.Source = "goldfish.png";
            // Gold
            optionsPicker.FontSize = 30;
            fishRow.IsVisible = true;


            typeText.Text = "Temperate Fish, Freshwater Fish";
            lifeText.Text = "10 - 15 Years";
            tempText.Text = "22.2 - 25.5°C";
            turbidText.Text = ">5 NTU ";
            phText.Text = "6.5 - 7.5";
            tdsText.Text = "170 - 250 ppm ";

            varText.Text = "Fantail, Jikin, Meteor etc.";
        }
        else if (selectedOption == "Guppy")
        {
            fisher.Source = "guppy.png";
            // light Green
            optionsPicker.FontSize = 30;
            fishRow.IsVisible = true;


            typeText.Text = "Tropical Fish, Freshwater Fish";
            lifeText.Text = "2 - 3 Years";
            tempText.Text = "23 - 28°C";
            turbidText.Text = ">5 NTU ";
            phText.Text = "7 - 8";
            tdsText.Text = "150 - 200 ppm ";

            varText.Text = "Comet, Tuxedo, Wagtail etc.";

        }
        else if (selectedOption == "Molly")
        {
            fisher.Source = "molly.png";
            // light Green
            optionsPicker.FontSize = 30;
            fishRow.IsVisible = true;


            typeText.Text = "Tropical Fish, Freshwater Fish";
            lifeText.Text = "3 - 5 Years";
            tempText.Text = "24 - 27°C";
            turbidText.Text = ">5 NTU ";
            phText.Text = "7.5 - 8.5";
            tdsText.Text = "200 - 300 ppm ";

            varText.Text = "Balloon, Black, Dalmation etc.";

        }
        else if (selectedOption == "Platy")
        {
            fisher.Source = "platy.png";
            // light Green
            optionsPicker.FontSize = 30;
            fishRow.IsVisible = true;


            typeText.Text = "Tropical Fish, Freshwater Fish";
            lifeText.Text = "3 - 4 Years";
            tempText.Text = "15 - 28°C";
            turbidText.Text = ">5 NTU ";
            phText.Text = "6.8 - 8.5";
            tdsText.Text = "200 - 300 ppm ";

            varText.Text = "Balloon Belly, Black, Dalmation etc.";

        }
        else if (selectedOption == "Tetra")
        {
            fisher.Source = "tetrafish.png";
            // Optionally, set a background color for Tetra
            optionsPicker.FontSize = 30;
            fishRow.IsVisible = true;


            typeText.Text = "Tropical Fish, Freshwater Fish";
            lifeText.Text = "5 - 10 Years";
            tempText.Text = "22 - 26°C";
            turbidText.Text = ">5 NTU ";
            phText.Text = "6.8 - 7.8";
            tdsText.Text = "130 - 195 ppm ";

            varText.Text = "Neon, Cardinal, Ember etc.";
        }
    }
}

