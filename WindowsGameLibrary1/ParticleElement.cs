using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Extension;
namespace VisualEffects
{
    #region ParticleElementクラス
    /// <summary>
    /// パーティクルの一粒を表すクラス
    /// </summary>
    public class ParticleElement
    {
        #region フィールド
        /// <summary>
        /// 描画に使うGraphicsDevice
        /// </summary>
        GraphicsDevice graphicsDevice;
        /// <summary>
        /// パーティクルに貼るテクスチャ1
        /// </summary>
        Texture2D texture1;
        /// <summary>
        /// パーティクルに貼るテクスチャ2(null可)
        /// </summary>
        Texture2D texture2;
        /// <summary>
        /// 座標と速度と加速度
        /// </summary>
        Vector3 position;
        Vector3 speed;
        Vector3 acceleration;
        /// <summary>
        /// 最初の大きさ
        /// </summary>
        //float sizeStart;
        /// <summary>
        /// 消える直前の大きさ
        /// </summary>
        //float sizeEnd;
        /// <summary>
        /// この粒の寿命(フレーム数)
        /// </summary>
        int lifeTime;
        /// <summary>
        /// 生きているフレーム数
        /// </summary>
        int frames;
        /// <summary>
        /// 回転
        /// </summary>
        int rotation;
        /// <summary>
        /// 粒の回転速度
        /// </summary>
        int rotsPerFrame;
        /// <summary>
        /// テクスチャ平面の法線方向
        /// </summary>
        Vector3 normal;
        #endregion

        static Random rand;


        #region コンストラクタ
        static ParticleElement()
        {
            rand = new Random();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="cameraVector"></param>
        /// <param name="tex1"></param>
        /// <param name="iniPosition"></param>
        /// <param name="sizeStart"></param>
        public ParticleElement(GraphicsDevice gDevice, Texture2D tex, Texture2D tex2,int life, Vector3 iniPosition, Vector3 spd, Vector3 accel,int rotsPerFrame,int rot = 0)
        {
            graphicsDevice = gDevice;
            texture1 = tex;
            texture2 = tex2;
            position = iniPosition;
            speed = spd;
            acceleration = accel;
            //this.sizeStart = sizeStart;
            //this.sizeEnd = sizeEnd;
            lifeTime = life;
            frames = 0;
            this.rotsPerFrame = rotsPerFrame;
            rotation = rot;

            Normal = Vector3.Zero;
            DrawOrder = 0;
            //Effect初期化
            //effect = new BasicEffect(graphicsDevice);
            //effect.InitialTexture = texture;
            //effect.TextureEnabled = true;
            //effect.EnableDefaultLighting();
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 寿命を迎えたかどうか
        /// </summary>
        public bool EndLife { get { return frames > lifeTime; } }
        /// <summary>
        /// 描画に必要な4つの点
        /// </summary>
        public VertexPositionNormalTexture[] Vertices { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        //public static short[] Indices
        //{
        //    get
        //    {
        //        return new short[]{
        //               0,1,2,
        //               2,1,3
        //            };
        //    }
        //}
        public static short[] Indices
        {
            get
            {
                List<short> il = new List<short>();
                for (short idx = 0; idx < (N + 1) * (N + 1) - 1 - (N + 1); idx++)
                {
                    if (idx % (N + 1) != N)
                    {
                        il.Add(idx);
                        il.Add((short)(idx + 1));
                        il.Add((short)(idx + N + 1));
                        
                        il.Add((short)(idx + N + 2));
                        il.Add((short)(idx + N + 1));
                        il.Add((short)(idx + 1));
                    }
                }
                return il.ToArray();
            }
        }
        public static Vector2 GetTextCoordinate(int n,int index)
        {
            return new Vector2();
        }

        /// <summary>
        /// 寿命が進んでいる割合
        /// </summary>
        public float Alpha
        {
            get { return (float)frames / lifeTime; }
        }
        /// <summary>
        /// この粒に貼るテクスチャ
        /// </summary>
        public Texture2D InitialTexture { get { return texture1; } }
        /// <summary>
        /// 変化後のテクスチャ(null可)
        /// </summary>
        public Texture2D FinalTexture { get { return texture2; } }
        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Speed { get { return speed; } set { speed = value; } }
        public Vector3 Acceleration { get { return acceleration; } set { acceleration = value; } }
        public Vector3 Color { get; set; }
        public float Size { get; set; }

        
        public Vector3 Normal { get; set; }
        public float DrawAlpha { get; set; }
        public Matrix CameraView { get; set; }
        public Matrix CameraProjection { get; set; }
        public int DrawOrder { get; set; }
        public bool EnableLighting {get; set; }
        //public bool Visible { get; set; }
        #endregion

        #region 更新
        public void Update(Vector3 cameraVector)
        {
            position += speed;
            speed += acceleration;
            //★全部決め打ちしてる
            //
            setVertices(cameraVector);
            rotation += rotsPerFrame;
            frames++;
        }

        #endregion

        #region 操作
        /// <summary>
        /// 指定したベクトルに加速
        /// </summary>
        /// <param name="vec"></param>
        public void Attract(Vector3 vec)
        {
            this.speed += vec;
        }
        BoundingSphere getSphere(float size) { return new BoundingSphere(position, size); } 
        
        public bool Hit(ParticleElement p)
        {
            if (Size / 2 > 0 && p.Size / 2 > 0)
            {
                return getSphere(Size / 2).Intersects(getSphere(p.Size / 2));
            }
            return false;
        }
        public float GetDistance(ParticleElement element)
        {
            return (Position - element.Position).Length() - (Size/2+element.Size/2);
        }
        #endregion
        //テクスチャが正方形でないときに
        //正方形のパーティクルができるのを直す
        private void setVertices()
        {
            float size = Size / 2; //MathHelper.Lerp(sizeStart, sizeEnd, Alpha) / 2;
            //★
            Vector3[] vecs;//offsetです
            if (texture1.Width >= texture1.Height)
            {
                float height = (float)texture1.Height / texture1.Width * size;
                vecs = new Vector3[]{
                    new Vector3(-size,size,0),
                    new Vector3(size,size,0),
                    new Vector3(-size,-size,0),
                    new Vector3(size,-size,0)
                };
            }
            else
            {
                float width = (float)texture1.Width / texture1.Height * size;
                vecs = new Vector3[]{
                    new Vector3(-size,size,0),
                    new Vector3(size,size,0),
                    new Vector3(-size,-size,0),
                    new Vector3(size,-size,0)
                };
            }
            for (int i = 0; i < vecs.Length; i++)
            {
                vecs[i] = new Vector3(
                    (float)(vecs[i].X * Math.Cos(MathHelper.ToRadians(rotation)) + vecs[i].Y * Math.Sin(MathHelper.ToRadians(rotation))),
                    (float)(vecs[i].X * -Math.Sin(MathHelper.ToRadians(rotation)) + vecs[i].Y * Math.Cos(MathHelper.ToRadians(rotation))),
                    vecs[i].Z
                );
            }

            //点をカメラに合わせて回転
            //回転角度を求める
            float rx, ry;
            turn(normal, out rx, out ry);
            //回転
            for (int i = 0; i < vecs.Length; i++)
            {
                vecs[i] = Vector3.Transform(vecs[i], Matrix.CreateRotationX(rx) * Matrix.CreateRotationY(ry));
            }

            //★点を増やす
            //★テクスチャコーディネート
            //割り方を関数にしておく
            //normalはビルボード用にも
            //ここでnormalを計算して各頂点に
            //vecsは中心から隅っこまでのoffset
            List<VertexPositionNormalTexture> vl = new List<VertexPositionNormalTexture>();
            for (int b = 0; b <= N; b++)
            {
                for (int a = 0; a <= N; a++)
                {
                    float angle = MathHelper.ToRadians(90);
                    float x = (float)a / N;
                    float y = (float)b / N;
                    Vector3 offset = new Vector3(x-0.5f,y-0.5f,0);//= new Vector3((float)a / N-0.5f,(float)b / N-0.5f,0);
                    
                    offset = Vector3.Transform(offset, Matrix.CreateRotationZ(MathHelper.ToRadians(this.rotation)) * Matrix.CreateRotationX(rx) * Matrix.CreateRotationY(ry));
                    //Vector3 norm = Vector3.TransformNormal(Vector3.Forward,Matrix.CreateRotationX(angle*offset.Y) * Matrix.CreateRotationY(angle*offset.X));
                    Vector3 norm = getNorm(normal, angle, offset);
                    //if(a % 2 == 1)
                    //    vl.Add(new VertexPositionNormalTexture(position + offset * -size, normal, new Vector2((float)a / (float)N, (float)b / (float)N)));
                    //else vl.Add(new VertexPositionNormalTexture(position + offset*size,normal,new Vector2((float)a / (float)N,(float)b / (float)N)));
                    vl.Add(new VertexPositionNormalTexture(position + offset * Size, norm, new Vector2((float)a / (float)N, (float)b / (float)N)));
                }
            }
            Vertices = vl.ToArray();
            //Vertices = new VertexPositionNormalTexture[]{
            //        new VertexPositionNormalTexture(position + vecs[0], normal, new Vector2(0, 0)),
            //        new VertexPositionNormalTexture(position + vecs[1], normal, new Vector2(1, 0)),
            //        new VertexPositionNormalTexture(position + vecs[2], normal, new Vector2(0, 1)),
            //        new VertexPositionNormalTexture(position + vecs[3], normal, new Vector2(1, 1))
            //    };
        }
        public static readonly int N = 2;
        private void setVertices(Vector3 newNormal)
        {
            //★法線方向がカメラの方向依存
            normal = Normal != Vector3.Zero ? Normal : Vector3.Normalize(newNormal);
            setVertices();
        }
        private Vector3 getNorm(Vector3 normal, float rad, Vector3 offset)
        {
            Vector3 di = offset;
            Vector3 n = normal + di;
            return Vector3.Normalize(n);
            /*Vector3 dir;
            if (normal.X != 0)
            {
                dir = new Vector3((-normal.Y - normal.Z) / normal.X, 1, 1);
            }
            else if (normal.Y != 0)
            {
                dir = new Vector3(1, (-normal.X - normal.Z) / normal.Y, 1);
            }
            else if (normal.Z != 0)
            {
                dir = new Vector3(1, 1, (-normal.X - normal.Y) / normal.Z);
            }
            else throw new ArgumentException("方向ベクトルが無効です");
            dir = Vector3.Normalize(dir);
            return Vector3.TransformNormal((float)Math.Cos(rad) * normal + (float)Math.Sin(rad) * dir, Matrix.CreateFromAxisAngle(normal, rad));
            */
        }
        private void turn(Vector3 cameraVector, out float radX, out float radY)
        {
            Vector2 vec = -new Vector2(cameraVector.X, cameraVector.Z);
            radY = vec.ToRadians();
            radX = (float)Math.Asin(cameraVector.Y / cameraVector.Length());
        }
    }
    #endregion

}
