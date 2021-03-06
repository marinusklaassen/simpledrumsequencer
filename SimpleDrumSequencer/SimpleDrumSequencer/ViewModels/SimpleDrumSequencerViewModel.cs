﻿using SimpleDrumSequencer.Models;
using SimpleDrumSequencer.Services;
using SimpleDrumSequencer.Utility;
using SimpleDrumSequencer.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleDrumSequencer.ViewModels
{
    public class SimpleDrumSequencerViewModel : ViewModelBase
    {
        public ISimpleDrumSequencerService SimpleDrumSequencerService;

        bool isRunning = false;
        public bool IsRunning
        {
            get { return isRunning; }
            set { SetProperty(ref isRunning, value); }
        }

        double volume = 0.8;
        public double Volume
        {
            get { return volume; }
            set
            {
                SetProperty(ref volume, value);
                OnVolumeChanged(volume);
            }
        }

        int sequencerPosition = 0;
        public int SequencerPosition
        {
            get { return sequencerPosition; }
            set { SetProperty(ref sequencerPosition, value); }
        }

        string lastPlayedInstrumentName = String.Empty;
        public string LastPlayedInstrumentName
        {
            get { return lastPlayedInstrumentName; }
            set { SetProperty(ref lastPlayedInstrumentName, value); }
        }

        string lastPlayedInstrumentNameShort = String.Empty;
        public string LastPlayedInstrumentNameShort
        {
            get { return lastPlayedInstrumentNameShort; }
            set { SetProperty(ref lastPlayedInstrumentNameShort, value); }
        }

        ObservableCollection<SequencerLaneModel> sequencerLanes;
        public ObservableCollection<SequencerLaneModel> SequencerLanes
        {
            get { return sequencerLanes; }
            set { SetProperty(ref sequencerLanes, value); }
        }

        public SimpleDrumSequencerViewModel(ISimpleDrumSequencerService simpleDrumSequencerService)
        {
            SimpleDrumSequencerService = simpleDrumSequencerService;

            RandomizeCommand = new Command(OnRandomizeCommand);
            StartCommand = new Command(OnStartCommand);
            StopCommand = new Command(OnStopCommand);
            PlaySoundCommand = new Command<SequencerLaneModel>(OnPlaySoundCommand);
            ResetCommand = new Command(OnResetCommand);
            
            string currentDrumKitFolder = "SimpleDrumSequencer.Audio.DrumKit.";

            SequencerLanes = SimpleDrumSequencerService.SequencerLanes;

            SimpleDrumSequencerService
            .AddInstrument("Bass Drum 1", "BD1", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Kick 01.wav"))
            .AddInstrument("Bass Drum 2", "BD2", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Kick 02.wav"))
            .AddInstrument("Snare 1", "SN1", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Snare 01.wav"))
            .AddInstrument("Snare 2", "SN2", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Snare 02.wav"))
            .AddInstrument("Closed Hat 1", "HH1", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Closed Hat 01.wav"))
            .AddInstrument("Closed Hat 2", "HH2", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Closed Hat 02.wav"))
            .AddInstrument("Open Hat 2", "OH", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Open Hat 01.wav"))
            .AddInstrument("Cymbal", "CB", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Cymbal 01.wav"))
            .AddInstrument("Low Tom", "LT", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Tom 01.wav"))
            .AddInstrument("Mid Tom", "MT", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Tom 02.wav"))
            .AddInstrument("High Tom", "HT", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Tom 03.wav"))
            .AddInstrument("Shaker", "SH", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Shaker 01.wav"))
            .AddInstrument("Clap", "CP", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Clap.wav"))
            .AddInstrument("Cowbell", "CB", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Cowbell 01.wav"))
            .AddInstrument("Zap", "ZP", FileLocator.GetFileStreamFromAssembly(currentDrumKitFolder + "Zap 01.wav"));

            SimpleDrumSequencerService.PositionChanged += OnSequencerPositionChanged;
        }

        public void OnSequencerPositionChanged(object sender, Utility.PositionChangedEventArgs e)
        {
            SequencerPosition = e.Position;
        }

        public void OnRandomizeCommand()
        {
            SimpleDrumSequencerService.Randomize();
        }

        public void OnStartCommand()
        {
            IsRunning = SimpleDrumSequencerService.Start().IsRunning;
        }

        public void OnStopCommand()
        {
            IsRunning = SimpleDrumSequencerService.Stop().IsRunning;
        }

        public void OnPlaySoundCommand(SequencerLaneModel sequencerLane)
        {
            LastPlayedInstrumentName = sequencerLane.InstrumentName;
            LastPlayedInstrumentNameShort = sequencerLane.InstrumentNameShort;
            Task.Run(() => { sequencerLane.AudioPlayer.Play(); });
        }

        public void OnResetCommand()
        {
            SimpleDrumSequencerService.Reset();
        }

        public void OnVolumeChanged(double volume)
        {
            SimpleDrumSequencerService.SetVolume(volume);
        }

        public ICommand RandomizeCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand PlaySoundCommand { get; }
        public ICommand ResetCommand { get; }

    }
}
