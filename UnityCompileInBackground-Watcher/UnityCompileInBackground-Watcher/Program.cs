using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UnityCompileInBackground_Watcher
{
	public static class Program
	{
		//==============================================================================
		// 変数
		//==============================================================================
		private static int m_waitTime;

		//==============================================================================
		// 関数
		//==============================================================================
		/// <summary>
		/// エントリポイント
		/// </summary>
		private static void Main( string[] args )
		{
			var options	= new string[] { "-p", "-w" };
			var result	= options.ToDictionary( c => c.Substring( 1 ), c => args.SkipWhile( a => a != c ).Skip( 1 ).FirstOrDefault() );
			var path	= result[ "p" ];

			m_waitTime = int.Parse( result[ "w" ] );

			// 非同期でファイルの監視を開始する
			var notifyFilter	=
				NotifyFilters.LastAccess	|
				NotifyFilters.LastWrite		|
				NotifyFilters.FileName		|
				NotifyFilters.DirectoryName	;

			var watcher = new FileSystemWatcher( path, "*.cs" )
			{
				NotifyFilter			= notifyFilter	,
				IncludeSubdirectories	= true			,
			};

			watcher.Changed += OnChanged;
			watcher.Created += OnChanged;
			watcher.Deleted += OnChanged;
			watcher.Renamed += OnRenamed;

			watcher.EnableRaisingEvents = true;

			// 非同期で監視を続けるために無限ループに入る
			while ( true ) { }
		}

		/// <summary>
		/// ファイルに変更があった時に呼び出されます
		/// </summary>
		private static async void OnChanged( object sender, FileSystemEventArgs e )
		{
			await Task.Delay( m_waitTime );
			Console.WriteLine( "OnChanged" );
		}

		/// <summary>
		/// ファイルの名前が変更された時に呼び出されます
		/// </summary>
		private static async void OnRenamed( object sender, RenamedEventArgs e )
		{
			await Task.Delay( m_waitTime );
			Console.WriteLine( "OnRenamed" );
		}
	}
}
