﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    public sealed class PacketBuffer : IDisposable
    {
        private readonly List<byte> _bufferList;
        private byte[] _readBuffer;
        private int _readPos;
        private bool _buffUpdate;

        public PacketBuffer()
        {
            _bufferList = new List<byte>();
            _readPos = 0;
        }

        public int GetReadPos()
        {
            return _readPos;
        }

        public byte[] ToArray()
        {
            return _bufferList.ToArray();
        }

        private int Count()
        {
            return _bufferList.Count;
        }

        public int Length()
        {
            int result = Count() - _readPos;
            return result >= 0 ? result : 0;
        }

        public void Clear()
        {
            _bufferList.Clear();
            _readPos = 0;
        }
        
        // Write Data
        public void WriteByte(byte input)
        {
            _bufferList.Add(input);
            _buffUpdate = true;
        }
        
        public void WriteBytes(byte[] input)
        {
            _bufferList.AddRange(input);
            _buffUpdate = true;
        }
        
        public void WriteInteger(int input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }
        
        public void WriteFloat(float input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }
        
        public void WriteString(string input)
        {
            _bufferList.AddRange(BitConverter.GetBytes(input.Length));
            _bufferList.AddRange(Encoding.ASCII.GetBytes(input));
            _buffUpdate = true;
        }
        
        // Read Data
        public byte ReadyByte(bool peek = true)
        {
            if (_bufferList.Count > _readPos)
            {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                byte value = _readBuffer[_readPos];
                if (peek && _bufferList.Count > _readPos)
                {
                    _readPos += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("Buffer is past its limit.");
            }
        }
        
        public byte[] ReadyBytes(int length ,bool peek = true)
        {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                byte[] value = _bufferList.GetRange(_readPos, length).ToArray();
                if (peek && _bufferList.Count > _readPos)
                {
                    _readPos += length;
                }

                return value;
            
        }
        
        public int ReadInteger(bool peek = true)
        {
            if (_bufferList.Count > _readPos)
            {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                int value = BitConverter.ToInt32(_readBuffer, _readPos);
                if (peek && _bufferList.Count > _readPos)
                {
                    _readPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("Buffer is past its limit.");
            }
        }
        
        public float ReadFloat(bool peek = true)
        {
            if (_bufferList.Count > _readPos)
            {
                if (_buffUpdate)
                {
                    _readBuffer = _bufferList.ToArray();
                    _buffUpdate = false;
                }

                float value = BitConverter.ToSingle(_readBuffer, _readPos);
                if (peek && _bufferList.Count > _readPos)
                {
                    _readPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("Buffer is past its limit.");
            }
        }
        
        public string ReadString(bool peek = true)
        {
            int length = ReadInteger();
            if (_buffUpdate)
            {
                _readBuffer = _bufferList.ToArray();
                _buffUpdate = false;
            }

            string value = Encoding.ASCII.GetString(_readBuffer, _readPos, length);
            if (peek && _bufferList.Count > _readPos)
            {
                _readPos += length;
            }

            return value;
        }
        
        // IDisposable
        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _bufferList.Clear();
                }

                _readPos = 0;
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}