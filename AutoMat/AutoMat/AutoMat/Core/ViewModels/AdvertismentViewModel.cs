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
    }
}
