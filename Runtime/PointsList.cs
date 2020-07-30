using HeatmapParticles.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeatmapParticles
{
    public class PointsList
    {

        static PointsList _instance = null;
        public static PointsList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PointsList();
                return _instance;
            }
        }

        const string toFormat = "Heatmap {0} out of {1}\n";

        public PointsList()
        {
            currIndex = 0;
            points = new List<ListContainer>();
            points.Add(new ListContainer
            {
                listTime = DateTime.Now,
                dictionary = new Dictionary<SmallVector3, HeatmapInfo>()
            });
            if(CurrDict == null)
            {
                Debug.Log("Ya done fucked up");
            }
        }

        public Dictionary<SmallVector3, HeatmapInfo> CurrDict { get => points[currIndex].dictionary; }

        public List<ListContainer> points;
        private int currIndex = 0;       

        public void Add(SmallVector3 point)
        {
            if (points == null)
            {
                points = new List<ListContainer>();
                
            }

            if(points.Count <= currIndex)
            {
                points.Add(new ListContainer
                {
                    dictionary = new Dictionary<SmallVector3, HeatmapInfo>(),
                    listTime = DateTime.Now
                });
            }

            if(points[currIndex].dictionary.TryGetValue(point, out HeatmapInfo info))
            {
                info.height++;
                //Adjust other heatmap info here
                points[currIndex].dictionary[point] = info;
            }
            else
            {
                points[currIndex].dictionary[point] = new HeatmapInfo
                {
                    //Adjust other heatmap info here
                    height = 1
                };
            }

        }

        public void ClearCurrent()
        {
            if (points == null) return;
            if (points.Count == 0 || points[currIndex].dictionary == null) return;
            else points[currIndex].dictionary.Clear();
        }

        public void ClearAll()
        {
            if (points == null) return;
            points.Clear();
        }

        public int CountCurrent
        {
            get
            {
                if (points == null)
                {
                    points = new List<ListContainer>();
                    points.Add(new ListContainer
                    {
                        dictionary = new Dictionary<SmallVector3, HeatmapInfo>(),
                        listTime = DateTime.Now
                    });
                }
                if (points[currIndex].dictionary == null)
                {

                }
                return points[currIndex].dictionary.Count;
            }
        }

        public void MoveUp()
        {
            currIndex++;
            if (currIndex >= points.Count)
            {
                points.Add(new ListContainer
                {
                    dictionary = new Dictionary<SmallVector3, HeatmapInfo>(),
                    listTime = DateTime.Now
                });
                currIndex = points.Count - 1;
            }
            if (points[currIndex].dictionary == null)
            {
                InitializeCurrElement();
            }
        }

        public void MoveDown()
        {
            currIndex--;
            if (currIndex < 0) currIndex = points.Count - 1;
            if (points[currIndex].dictionary == null)
            {
                InitializeCurrElement();
            }
        }

        public void ResetTime()
        {
            if (points[currIndex].dictionary == null)
            {
                InitializeCurrElement();
            }
            else
            {
                ListContainer container = points[currIndex];
                container.listTime = DateTime.Now;
                points[currIndex] = container;
            }

        }

        public void InitializeCurrElement()
        {
            if(points.Count == 0)
            {
                points.Add(new ListContainer
                {

                });
            }
            points[currIndex] = new ListContainer
            {
                dictionary = new Dictionary<SmallVector3, HeatmapInfo>(),
                listTime = DateTime.Now
            };
        }

        public void Save(DataWriter writer)
        {
            writer.Write(points.Count);
            writer.Write(currIndex);
            for(int i = 0; i < points.Count; i++)
            {
                points[i].Save(writer);
            }
        }

        public void Load(DataReader reader)
        {
            points = new List<ListContainer>();
            int count = reader.ReadInt();
            currIndex = reader.ReadInt();
            for(int i = 0; i < count; i++)
            {
                points.Add(ListContainer.Load(reader));
                
            }

        }

        public string GetInfo()
        {
            return string.Format(toFormat, currIndex + 1, points.Count);
        }

        public string GetDateTime()
        {
            return points[currIndex].listTime.ToShortDateString() + " " + points[currIndex].listTime.ToLongTimeString();
        }

    }


    [System.Serializable]
    public struct HeatmapInfo
    {
        public int height;

        public void Save(DataWriter writer)
        {
            writer.Write(height);
        }

        public static HeatmapInfo Load(DataReader reader)
        {
            return new HeatmapInfo
            {
                height = reader.ReadInt()
            };
        }
    }

    [System.Serializable]
    public struct ListContainer
    {
        public Dictionary<SmallVector3, HeatmapInfo> dictionary;

        public DateTime listTime;

        public void Save(DataWriter writer)
        {
            writer.Write(dictionary.Count);
            foreach(KeyValuePair<SmallVector3, HeatmapInfo> pair in dictionary)
            {
                pair.Key.Save(writer);
                pair.Value.Save(writer);
            }
            writer.Write(listTime);
        }

        public static ListContainer Load(DataReader reader)
        {
            Dictionary<SmallVector3, HeatmapInfo> dict = new Dictionary<SmallVector3, HeatmapInfo>();
            int pointCount = reader.ReadInt();
            dict.Clear();
            for (int j= 0; j < pointCount; j++)
            {
                SmallVector3 key = SmallVector3.Load(reader);
                HeatmapInfo value = HeatmapInfo.Load(reader);
                if (dict.ContainsKey(key)) dict[key] = value;
                else dict.Add(key, value);
            }
            return new ListContainer
            {
                dictionary = dict,
                listTime = reader.ReadDateTime()
            };
        }
    }

}
