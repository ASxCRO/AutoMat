using AutoMat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace AutoMat.Core.ViewModels
{
    public class AdvertismentViewModel : BaseViewModel<FileImageSource>
    {
        public AdvertismentViewModel()
        {
            Advertisment = new Advertisement();
            _imageSources = new ObservableCollection<ImageSource>();
            ImageSources = new ObservableCollection<ImageSource>();
        }

        public Advertisement Advertisment{ get; set; }

        public int _currentImageIndex;
        public int CurrentImageIndex
        {
            get { return _currentImageIndex; }
            set
            {
                _currentImageIndex = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ImageSource> _imageSources;
        public ObservableCollection<ImageSource> ImageSources
        {
            get { return _imageSources; }
            set
            {
                _imageSources = value;
                OnPropertyChanged();
            }
        }

        string heightList;
        int heightRowsList = 80;
        public string AEHeightList
        {
            get
            {
                heightList = (Advertisment.AdditionalEquipment.Count * heightRowsList).ToString();
                return heightList;
            }
        }
        public string SEHeightList
        {
            get
            {
                heightList = (Advertisment.SafetyEquipment.Count * heightRowsList).ToString();
                return heightList;
            }
        }

        public string REHeightList
        {
            get
            {
                heightList = (Advertisment.RadioEquipment.Count * heightRowsList).ToString();
                return heightList;
            }
        }

        public string ACEHeightList
        {
            get
            {
                heightList = (Advertisment.ACEquipment.Count * heightRowsList).ToString();
                return heightList;
            }
        }

        public string PTHeightList
        {
            get
            {
                heightList = (Advertisment.PayTypes.Count * heightRowsList).ToString();
                return heightList;
            }
        }

        public string CEHeightList
        {
            get
            {
                heightList = (Advertisment.ComfortEquipment.Count * heightRowsList).ToString();
                return heightList;
            }
        }
    }
}
