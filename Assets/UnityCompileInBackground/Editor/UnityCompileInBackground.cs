using System.Diagnostics;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace KoganeUnityLib
{
	/// <summary>
	/// バックグラウンドでコンパイルを開始するためのエディタ拡張
	/// </summary>
	public static class UnityCompileInBackground
	{
		//==============================================================================
		// 定数
		//==============================================================================
		private const string CONSOLE_APP_PATH = @"UnityCompileInBackground/Editor/UnityCompileInBackground-Watcher.exe";

		//==============================================================================
		// 変数
		//==============================================================================
		private static Process	m_process	;	// ファイル監視ツールのプロセス
		private static bool		m_isRefresh	;	// コンパイルを開始する場合 true

		//==============================================================================
		// 関数
		//==============================================================================
		/// <summary>
		/// Unity エディタが起動した時に呼び出されます
		/// </summary>
		[InitializeOnLoadMethod]
		private static void Init()
		{
			// ファイル監視ツールを起動します
			// ツールからメッセージを受信したらコンパイルを開始します
			var dataPath	= Application.dataPath;
			var filename	= dataPath + "/" + CONSOLE_APP_PATH;
			var path		= Application.dataPath;
			var arguments	= string.Format( @"-p ""{0}"" -w 0", path );
			var windowStyle	= ProcessWindowStyle.Hidden;

			var info = new ProcessStartInfo
			{
				FileName				= filename		,
				UseShellExecute			= false			,
				RedirectStandardOutput	= true			,
				CreateNoWindow			= true			,
				WindowStyle				= windowStyle	,
				Arguments				= arguments		,
			};

			m_process = Process.Start( info );
			m_process.OutputDataReceived += OnReceived;
			m_process.BeginOutputReadLine();

			UnityEngine.Debug.Log( "[UnityCompileInBackground] Start Watching" );

			EditorApplication.update += OnUpdate;
			EditorApplication.quitting += OnQuit;
			CompilationPipeline.assemblyCompilationStarted += OnCompilationStarted;
		}

		/// <summary>
		/// Unity エディタが終了する時に呼び出されます
		/// </summary>
		private static void OnQuit()
		{
			Dispose();
		}

		/// <summary>
		/// コンパイルが開始した時に呼び出されます
		/// </summary>
		private static void OnCompilationStarted( string _ )
		{
			Dispose();
		}

		/// <summary>
		/// ファイル監視ツールを止めます
		/// </summary>
		private static void Dispose()
		{
			// すでに止まっている場合は何も行いません
			if ( m_process == null ) return;

			if ( !m_process.HasExited )
			{
				m_process.Kill();
			}
			m_process.Dispose();
			m_process = null;

			UnityEngine.Debug.Log( "[UnityCompileInBackground] Stop Watching" );
		}

		/// <summary>
		/// エディタの更新タイミングで呼び出されます
		/// </summary>
		private static void OnUpdate()
		{
			// コンパイルフラグが立っていない場合、コンパイル中の場合、
			// リフレッシュ中の場合はここで処理を止めます
			if ( !m_isRefresh ) return;
			if ( EditorApplication.isCompiling ) return;
			if ( EditorApplication.isUpdating ) return;

			// コンパイルを開始します
			UnityEngine.Debug.Log( "[UnityCompileInBackground] Start Compiling" );

			m_isRefresh = false;

			AssetDatabase.Refresh();
		}

		/// <summary>
		/// ファイル監視ツールからメッセージを受信した時に呼び出されます
		/// </summary>
		private static void OnReceived( object sender, DataReceivedEventArgs e )
		{
			var message = e.Data;

			// ファイルに変更があった場合もしくは
			// ファイルの名前が変更されたらコンパイルフラグを立てます
			//
			// この関数では AssetDatabase.Refresh を呼び出しても何も起きないので
			// フラグだけ立てておいて Refresh は EditorApplication.update で行います
			if ( message.Contains( "OnChanged" ) || message.Contains( "OnRenamed" ) )
			{
				m_isRefresh = true;
			}
		}
	}
}

