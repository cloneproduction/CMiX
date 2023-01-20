//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Collections.Specialized;
//using System.Linq;

//namespace CMiX.Core.Presentation.ViewModels.Beat
//{
//    public class MasterBeatContainer
//    {
//        private static MasterBeatContainer instance;

//        private MasterBeatContainer() { }

//        public static MasterBeatContainer Instance
//        {
//            get { return instance ?? (instance = new MasterBeatContainer()); }
//        }


//        private ObservableCollection<MasterBeat> _masterBeats;

//        public ObservableCollection<MasterBeat> MasterBeats
//        {
//            get 
//            { 
//                if(_masterBeats == null)
//                {
//                    _masterBeats = new ObservableCollection<MasterBeat>();
//                }

//                return _masterBeats; 
//            }
//        }

//        public MasterBeat GetMasterBeat(Guid id)
//        {
//            return MasterBeats.FirstOrDefault(x => x.ID == id);
//        }

//        public MasterBeat GetMasterBeat(int index)
//        {
//            return MasterBeats.ElementAt(index);
//        }

//        public void RemoveMasterBeat(Guid id)
//        {
//            var masterBeat = MasterBeats.FirstOrDefault(x => x.ID == id);
//            MasterBeats.Remove(masterBeat);
//        }

//        public void RemoveMasterBeat(MasterBeat masterBeat)
//        {
//            MasterBeats.Remove(masterBeat);
//        }

//        public void AddMasterBeat(MasterBeat masterBeat)
//        {
//            MasterBeats.Add(masterBeat);
//        }
//    }
//}
