////////////////////////////////////////////////////////////////////////////////////////////////////
//
// OOPDemo
// Copyright (c) Kouji Matsui (@kekyo2)
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPDemo
{
	// クラス分割（抽象化）の一例：

	// 実際に描画する何かのためのインターフェイス（GDI+とかOpenCVとか）
	public interface IDrawEngine
	{
		void DrawPoint(int x, int y);
		void DrawLine(int x0, int y0, int x1, int y1);
	}

#if !BASECLASS
	// 共通の基底クラス
	// （抽象メソッド定義が含まれている場合は、クラスも抽象クラスとなる）
	public abstract class ElementBase
	{
		// 継承しないと使えない
		protected ElementBase()
		{
		}

		// 「描画位置の移動」の抽象定義
		public abstract void MoveTo(int x, int y);

		// 「描画実行」の抽象定義
		public abstract void Draw(IDrawEngine engine);
	}
#else
	// 共通のインターフェイス
	// （抽象メソッドしかない場合は、インターフェイスとした方がベター）
	public interface ElementBase
	{
		// 「描画位置の移動」の抽象定義
		void MoveTo(int x, int y);
		// 「描画実行」の抽象定義
		void Draw(IDrawEngine engine);
	}
#endif

	// 点描画を実行する具象クラス
	// （点描画を行う実装（アルゴリズムなど）だけが、内部に隠蔽されている）
	public sealed class PointElement : ElementBase	// <-- 基底クラスを継承することで、MoveToとDrawが存在する事を定義
	{
		private int x_;
		private int y_;

		// 「描画位置の移動」の実装
		public override void MoveTo(int x, int y)
		{
			x_ = x;
			y_ = y;
		}

		// 「描画実行」の実装
		public override void Draw(IDrawEngine engine)
		{
			engine.DrawPoint(x_, y_);
		}
	}

	// 線描画を実行する具象クラス
	// （線描画を行う実装（アルゴリズムなど）だけが、内部に隠蔽されている）
	public sealed class LineElement : ElementBase	// <-- 基底クラスを継承することで、MoveToとDrawが存在する事を定義
	{
		private int lastX_;
		private int lastY_;
		private int x_;
		private int y_;

		// 「描画位置の移動」の実装
		public override void MoveTo(int x, int y)
		{
			lastX_ = x_;
			lastY_ = y_;

			x_ = x;
			y_ = y;
		}

		// 「描画実行」の実装
		public override void Draw(IDrawEngine engine)
		{
			engine.DrawLine(lastX_, lastY_, x_, y_);
		}
	}

	// 座標を格納するモデルクラス
	public sealed class Point
	{
		public readonly int X;
		public readonly int Y;

		public Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}

	class Program
	{
		// 座標列に従って描画を実行する
		private static void DrawByPoints(IDrawEngine engine, ElementBase drawer, Point[] points)
		{
			// 座標を列挙
			foreach (var point in points)
			{
				// 移動して
				drawer.MoveTo(point.X, point.Y);

				// 描画
				drawer.Draw(engine);
			}
		}

		// テストコード
		// （もちろん、IDrawEngineの実装がないので、このままでは実行はできない）
		private static void DrawTest(IDrawEngine engine)
		{
			// 座標列を生成
			var points = new[] { new Point(1, 1), new Point(5, 4), new Point(7, 3), new Point(7, 3) };

			/////////////////////////////////////////////////////////////////
			// 以下、ElementBaseを差し替えるだけで、処理内容を変える事が出来る

			// 座標列に従って、点を描画する
			DrawByPoints(engine, new PointElement(), points);

			// 座標列に従って、線を描画する
			DrawByPoints(engine, new LineElement(), points);

			// 「座標列に従って、（ほにゃほにゃ）を描画する」
			//   ... ためには、ElementBaseを継承した描画クラスを作ればよい事になる。
		}

		public static void Main()
		{
			
		}
	}
}
