using UnityEditor;
using UnityEngine;

namespace KoganeUnityLib
{
	[InitializeOnLoad]
	public static class UnityCompileInBackground
	{
		private const string NAME						= "UnityCompileInBackground";
		private const string KEY_IS_ENABLE				= NAME + "-IsEnable";
		private const string KEY_WAIT_TIME				= NAME + "-WaitTime";
		private const string KEY_IS_ENABLE_DEBUG_LOG	= NAME + "-IsEnableDebugLog";

		private static bool		m_isEnable			;
		private static bool		m_isEnableDebugLog	;
		private static float	m_waitTime			;
		private static double	m_time				;
		private static bool		m_isLoaded			;

		static UnityCompileInBackground()
		{
			m_time = EditorApplication.timeSinceStartup;
			EditorApplication.update += OnUpdate;
		}

		private static void Load()
		{
			if ( m_isLoaded ) return;

			m_isLoaded = true;

			m_isEnable			= EditorPrefs.GetBool( KEY_IS_ENABLE, true );
			m_waitTime			= EditorPrefs.GetFloat( KEY_WAIT_TIME, 1 );
			m_isEnableDebugLog	= EditorPrefs.GetBool( KEY_IS_ENABLE_DEBUG_LOG, false );
		}

		private static void OnUpdate()
		{
			Load();

			if ( !m_isEnable ) return;
			if ( EditorApplication.isCompiling ) return;
			if ( EditorApplication.isUpdating ) return;
			if ( EditorApplication.timeSinceStartup - m_time < m_waitTime ) return;

			m_time = EditorApplication.timeSinceStartup;
			AssetDatabase.Refresh();

			if ( !m_isEnableDebugLog ) return;

			Debug.LogFormat( "[ {0} ] Refresh", NAME );
		}

#if !UNITY_2018_3_OR_NEWER
		[PreferenceItem( "Compile in BG" )]
#endif
		private static void OnGUI()
		{
			Load();

			EditorGUI.BeginChangeCheck();

			m_isEnable			= EditorGUILayout.Toggle( "Enabled", m_isEnable );
			m_waitTime			= MultipleFloor( EditorGUILayout.Slider( "Wait Time ( sec )", m_waitTime, 0, 10 ), 0.1f );
			m_isEnableDebugLog	= EditorGUILayout.Toggle( "Enabled Debug Log", m_isEnableDebugLog );

			if ( EditorGUI.EndChangeCheck() )
			{
				EditorPrefs.SetBool( KEY_IS_ENABLE, m_isEnable );
				EditorPrefs.SetFloat( KEY_WAIT_TIME, m_waitTime );
				EditorPrefs.SetBool( KEY_IS_ENABLE_DEBUG_LOG, m_isEnableDebugLog );
			}
		}

#if UNITY_2018_3_OR_NEWER
		[SettingsProvider]
		public static SettingsProvider CreateSettingsProvider()
		{
			var path		= "Preferences/" + NAME;
			var provider	= new SettingsProvider( path, SettingsScope.User )
			{
				label		= NAME,
				guiHandler	= _ => OnGUI(),
				keywords	= new []{ NAME, "Enabled", "Wait Time ( sec )" },
			};

			return provider;
		}
#endif

		private static float MultipleFloor( float value, float multiple )
		{
			return Mathf.Floor( value / multiple ) * multiple;
		}
	}
}