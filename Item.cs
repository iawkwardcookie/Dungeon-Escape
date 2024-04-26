using System;
using System.Collections.Generic;

namespace Dungeon_Escape
{
    class Item : Board
    {
        protected int x, y;
        public int num; //the number that represents that item within 'boardLayout'
        public Coord GetCoord()
        {
            //return currently stored coordinate values//
            Coord position; //create coordinate
            position.x = x; //change coordinates to values previously stored
            position.y = y;
            return position; //return current position of object
        }
        public void SetCoord(Coord newPosition)
        {
            //change coordinates to new values//
            x = newPosition.x;
            y = newPosition.y;
        }
        public bool Collision(Coord otherObjectPosition)
        {
            //take x, y of object, compare with coordinates of another object//
            if (x == otherObjectPosition.x && y == otherObjectPosition.y)
                return true;
            else
                return false;
        }
    }

    class Character : Item
    {
        /// <summary>
        /// takes direction, moves object and returns true if on a key
        /// </summary>
        public bool ItemMovement(char direction, Keys keys)
        {
            bool nowOnKey = false;
            bool wasOnKey = false;

            // store coordinate before movement //
            Coord prevCoord = GetCoord();

            // check if legal move, then adjust coordinate //
            if (LegalMove(direction, prevCoord))
            {
                switch (direction)
                {
                    case 'N':
                        y--;
                        break;
                    case 'E':
                        x++;
                        break;
                    case 'S':
                        y++;
                        break;
                    case 'W':
                        x--;
                        break;
                }

                if (keys.FindKeyIndex(GetCoord()) != -1)
                    nowOnKey = true;

                if (keys.FindKeyIndex(prevCoord) != -1)
                    wasOnKey = true;

                // replace item placement on board //
                ReplaceItem(prevCoord, GetCoord(), num, wasOnKey, keys);
                return nowOnKey;
            }
            return nowOnKey;
        }
        private bool LegalMove(char direction, Coord pos)
        {
            switch (direction)
            {
                case 'N':
                    pos.y--;
                    break;
                case 'E':
                    pos.x++;
                    break;
                case 'S':
                    pos.y++;
                    break;
                case 'W':
                    pos.x--;
                    break;
            }
            if (Empty(pos))
                return true;
            else
                return false;
        }
        public bool onKey { get; set; }
    }
    class Enemy : Character
    {
        private bool startWandering;

        private List<char> path = new();
        public Enemy()
        {
            num = 5; //number used for the board layout 2d array
            Alerted = false;
            startWandering = true; //used to signal for the path to be recreated for wandering
            Visibility = 3; //visibility will be variable depending on enemy's anger
        }
        public int Visibility { get; set; } //0 to 3, invisible to fully visible
        public bool Alerted { get; set; }
        public char GetNextMove(Coord dest, bool wander)
        {
            if (wander && path.Count == 0) //if direction path list is empty, create new path with random destination
            {
                Coord rand = GetRandomCoord();
                path = PathFinding.FindPath(BoardLayout, y, x, rand.y, rand.x);
                startWandering = !startWandering;
            }
            else if (!wander)
                PathFinding.FindPath(BoardLayout, y, x, dest.y, dest.x);

            char move = path[0]; //return the next direction, removing it from the list
            path.RemoveAt(0);
            return move;
        }

        /*
        private int attentionSpan;
        private int turnsPerMove;
        private int visibility;
        private bool alerted;
        private bool startWander;
        private Coord wantedDestination;

        public Enemy(int turnsPerMove, int attentionSpan)
        {
            TurnsPerMove = turnsPerMove;
            SetAttention(attentionSpan);

            Visibility = 3;
            alerted = false;
            startWander = true;
        }
        private void SetAttention(int attention) => attentionSpan = attention; //the amount of turns it takes for enemy to lose interest in noise
        public int Visibility
        {
            get => visibility; //0 to 3, invisible to fully visible
            set => visibility = value;
        }
        public int TurnsPerMove
        {
            get => turnsPerMove; //the amount of turns it takes for the enemy to move (slowness)
            set => turnsPerMove = value;
        }
        public void NewAlert(Noise noise)
        {
            alerted = true;
            Visibility = 3;
            wantedDestination = noise.GetCoord();
        }
        public bool GetAlerted() => alerted;
        public void SetAlert(Noise noise) //when enemy first alerted, this method is run. Goes towards noise
        {
            alerted = true;
            Visibility = 3;
            wantedDestination = noise.GetCoord();
        }
        public bool GetStartWander() => startWander;
        public void SetStartWander(bool wander) => startWander = wander;
        public Coord WantedDestination
        {
            get => wantedDestination;
            set => wantedDestination = value;
        }
        */
    }
    class Player : Character
    {
        public Player()
        {
            num = 4;
        }
    }

    class Noise : Item
    {
        private int areaOfEffect;
        private bool noiseOccured;
        public bool NoiseOccured => noiseOccured;
        public void NewNoise(int cause)
        {
            SetAOE(cause);


        }
        public void SetAOE(int cause) =>
            //set intensity from cause type//
            areaOfEffect = cause switch
            {
                0 => 3, //footsteps
                1 => 5, //key
                2 => 9, //shout
                _ => 0,
            };
        public int GetAOE() => areaOfEffect;
    }
    class Key : Item
    {
        public Key()
        {
            num = 2;
        }
    }

    // all keys controlled in one class //
    class Keys : Board
    {
        private List<Tuple<Key,Character>> keysList = new(); //list of key coordinate, held status
        private readonly Character na = new(); //create empty character, for when key is not held

        public void CreateKeysList(params Key[] keys)
        {
            keysList = new();
            for (int i = 0; i < keys.Length; i++)
            {
                keysList.Add(Tuple.Create(keys[i], na));
                keys[i].SetCoord(SetItemPos(2));
                PlaceObject(keys[i].GetCoord(), keys[i].num);
            }
        }
        public int FindKeyIndex(Coord coord)
        {
            Coord keycoord;
            for (int i = 0; i < keysList.Count; i++)
            {
                keycoord = keysList[i].Item1.GetCoord(); //key's coordinate in keylist
                if (keycoord.x == coord.x && keycoord.y == coord.y) //if coordinate of key and character are equal, return that key
                {
                    return i;
                }
            }
            return -1; //if no key is in same cell as character, return -1
        }
        public List<int> GetHeldKeys()
        {
            int value;
            List<int> keys = new();
            for (int i = 0; i < keysList.Count; i++)
            {
                value = (keysList[i].Item2) switch
                {
                    Enemy => 0,
                    Player => 1,
                    _ => -1
                };
                keys.Add(value);
            }
            return keys;
        }
        public int HeldByWho(int key) 
        => (keysList[key].Item2) switch
        {
            Enemy => 0,
            Player => 1,
            _ => -1
        };
        public bool HoldingKeys(Character c)
        {
            for (int i = 0; i < keysList.Count; i++)
            {
                if (keysList[i].Item2 == c)
                {

                    return true;
                }
            }
            return false;
        }
        public Coord FindCoord(int key) => keysList[key].Item1.GetCoord();
        public bool PickKey(Character c)
        {
            Key key;
            Coord charCoord = c.GetCoord();
            int keyindex = FindKeyIndex(charCoord);

            if (keyindex!= -1)
            {
                key = keysList[keyindex].Item1;
                if (keysList[keyindex].Item2 != na)
                {
                    keysList.RemoveAt(keyindex);
                    keysList.Add(Tuple.Create(key, c));

                    AlterSideMenu(GetHeldKeys());
                    return true; //true returned if key is successfully picked up
                }
            }
            
            return false;
        }
        public bool DropKey(Character c)
        {
            Coord charCoord = c.GetCoord();
            int keyindex = FindKeyIndex(charCoord);
            Key key;
            Coord keyCoord;
            if (keyindex != -1 && HoldingKeys(c))
            {
                key = keysList[keyindex].Item1;
                if (keysList[keyindex].Item2 != na)
                {
                    keysList.RemoveAt(keyindex);
                    keyCoord = GetRandomCoordNear(key.GetCoord());

                    PlaceObject(keyCoord, key.num);
                    AlterSideMenu(GetHeldKeys());
                    return true;
                }
            }

            return false;
        }
        public int KeyCount => keysList.Count;
    }
}
