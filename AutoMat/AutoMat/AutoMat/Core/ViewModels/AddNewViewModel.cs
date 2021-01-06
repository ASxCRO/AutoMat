using AutoMat.Core.Constants;
using AutoMat.Core.Models;
using AutoMat.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace AutoMat.Core.ViewModels
{
    public class AddNewViewModel : BaseViewModel<FileImageSource>
    {
        public AddNewViewModel()
        {
            Init();
        }

        public void Init()
        {
            //string lists
            productionYearsStrings = new List<string>();
            countiesStrings = new List<string>();
            townsStrings = new List<string>();
            brandsStrings = new List<string>();
            modelsStrings = new List<string>();
            registeredUntilStrings = new List<string>();
            imageNames = new List<string>();

            imagesByteArrays = new List<Stream>();

            _imageSources = new ObservableCollection<FileImageSource>();
            ImageSources = new ObservableCollection<FileImageSource>();
            additionalEquipment = new AdditionalEquipment();
            selectedCarBrand = new CarBrand();
            Advertisement = new Advertisement();
            additionalEquipment = new AdditionalEquipment();
        }

        public ObservableCollection<FileImageSource> _imageSources;
        public ObservableCollection<FileImageSource> ImageSources
        {
            get { return _imageSources; }
            set
            {
                _imageSources = value;
                OnPropertyChanged();
            }
        }
        public AdditionalEquipment additionalEquipment { get; set; }
        public IEnumerable<County> counties { get; set; }
        public IEnumerable<Town> towns { get; set; }

        public IEnumerable<CarBrand> brands { get; set; }
        public IEnumerable<CarModel> models { get; set; }

        public List<string> productionYearsStrings { get; set; }
        public List<string> countiesStrings { get; set; }
        public List<string> townsStrings { get; set; }
        public List<string> brandsStrings { get; set; }
        public List<string> modelsStrings { get; set; }
        public List<string> registeredUntilStrings { get; set; }

        public List<string> imageNames { get; set;  }

        public List<Stream> imagesByteArrays { get; set; } 
        public CarBrand selectedCarBrand { get; set; }
        public IEnumerable<CarModel> filteredModels { get; set; }

        public Advertisement Advertisement { get; set; }

        public int AdvertismentEquipmentCount { get; set; }
        public int SecurityEquipmentCount { get; set; }
        public int ACEquipmentCount { get; set; }

        public int AirBagEquipmentCount { get; set; }

        public int StealEquipmentCount { get; set; }

        public int RadioEquipmentCount { get; set; }

        public int ComfortEquipmentCount { get; set; }

        public int PayTypesCount { get; set; }


        public string isEntered { get; set; } = "Unijeli ste";

        public string isNotEntered { get; set; } = "Niste još unijeli";



        public void SetUpStringLists()
        {
            foreach (var item in AdvertismentConstants.productionYearsNumber)
            {
                productionYearsStrings.Add(item.ToString());
            }

            foreach (var item in counties)
            {
                countiesStrings.Add(item.Name);
            }


            foreach (var item in towns)
            {
                townsStrings.Add(item.Name);
            }


            foreach (var item in brands)
            {
                brandsStrings.Add(item.Title.ToString());
            }

            for (int i = 0; i <= 12; i++)
            {
                var month = DateTime.Now.Month + i;
                var year = DateTime.Now.Year;
                if (month > 12)
                {
                    month -= 12;
                    year += 1;
                }

                registeredUntilStrings.Add($"{month}/{year}");
            }
        }

        public void SetUpStringListsForFilter()
        {
            foreach (var item in AdvertismentConstants.productionYearsNumber)
            {
                productionYearsStrings.Add(item.ToString());
            }

            foreach (var item in counties)
            {
                countiesStrings.Add(item.Name);
            }

            foreach (var item in brands)
            {
                brandsStrings.Add(item.Title.ToString());
            }
        }
    }
}
