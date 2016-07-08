// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot
{
    /// <summary>
    /// Configuration Multithreaded Singleton
    /// Using this, configuration attributes
    /// can be easily accessed from any place in solution.
    /// </summary>
    public partial class Configuration
    {
        /// <summary>
        /// Configuration Names Static Inner Class
        /// </summary>
        public static class Names
        {
            /// <summary>
            /// Key for current mode in configuration file
            /// </summary>
            public const string KEY_IS_DEMO_MODE = "IsDemoMode";

            /// <summary>
            /// Key for arduinos connection flag
            /// </summary>
            public const string KEY_IS_ARDUINOS_CONNECTED = "IsArduinosConnected";

            /// <summary>
            /// Key for Frame Width in capture device of Image Processing Unit
            /// </summary>
            public const string KEY_IPU_FRAME_WIDTH = "FrameWidth";

            /// <summary>
            /// Key for Frame Height in capture device of Image Processing Unit
            /// </summary>
            public const string KEY_IPU_FRAME_HEIGHT = "FrameHeight";

            /// <summary>
            /// Key for Frame Rate (FPS) in capture device of Image Processing Unit
            /// </summary>
            public const string KEY_IPU_FRAME_RATE = "FrameRate";

            /// <summary>
            /// Key for Table Width (Axe X) in mm
            /// </summary>
            public const string TABLE_WIDTH = "Width";

            /// <summary>
            /// Key for Table Height (Axe Y) in mm
            /// </summary>
            public const string TABLE_HEIGHT = "Height";

            /// <summary>
            /// Key for Ball Diameter in mm
            /// </summary>
            public const string BALL_DIAMETR = "BallDiameter";
            
            /// <summary>
            /// Key for Foosbot Axe X Size in Foosbot World in pts
            /// </summary>
            public const string FOOSBOT_AXE_X_SIZE = "axeX";

            /// <summary>
            /// Key for Foosbot Axe Y Size in Foosbot World in pts
            /// </summary>
            public const string FOOSBOT_AXE_Y_SIZE = "axeY";

            /// <summary>
            /// Key for Minimal Sector Width
            /// </summary>
            public const string TABLE_RODS_DIST = "MinSectorWidth";

            /// <summary>
            /// Key for Sector Calculation Factor (double)
            /// </summary>
            public const string SECTOR_FACTOR = "SectorFactor";
            
            /// <summary>
            /// Key for Foosbot System Delays (Mechanical and Networking)
            /// </summary>
            public const string FOOSBOT_DELAY = "TimeDelay";

            /// <summary>
            /// Key for Posible vector calculation Distance Error
            /// </summary>
            public const string VECTOR_CALC_DISTANCE_ERROR = "DistanceErrorTh";

            /// <summary>
            /// Key for Posible vector calculation Angle Error
            /// </summary>
            public const string VECTOR_CALC_ANGLE_ERROR = "AngleErrorTh";

            /// <summary>
            /// Key for Ricochet Factor (double)
            /// </summary>
            public const string KEY_RICOCHET_FACTOR = "RicochetFactor";

            /// <summary>
            /// Key for Minimal Rod Start Stopper position in mm 
            /// </summary>
            public const string KEY_ROD_START_Y = "RodStartY";

            /// <summary>
            /// Key for Maximal Rod End Stopper position in mm 
            /// </summary>
            public const string KEY_ROD_END_Y = "RodEndY";

            /// <summary>
            /// Key for player width in mm
            /// </summary>
            public const string KEY_PLAYER_WIDTH = "PlayerWidth";

            /// <summary>
            /// Key for Ball Intersection with Rods Maximum Time Relevant Prediction in seconds
            /// </summary>
            public const string KEY_ROD_INTERSECTION_MAX_TIMESPAN_SEC = "RodIntersectionMaxTimespan";

            /// <summary>
            /// Key for Dynamic Sector, [True] if in use, [False] otherwise
            /// </summary>
            public const string KEY_USE_DYNAMIC_SECTOR = "DynamicSector";

            /// <summary>
            /// Subkey for rod: Distance between 2 players on rod (in mm)
            /// </summary>
            public const string SUBKEY_DISTANCE = "_Distance";

            /// <summary>
            /// Subkey for rod: Count of players on rod (in mm)
            /// </summary>
            public const string SUBKEY_COUNT = "_Count";

            /// <summary>
            /// Subkey for rod: Distance from start stoper to First Player center (in mm)
            /// </summary>
            public const string SUBKEY_OFFSET_Y = "_OffsetY";

            /// <summary>
            /// Subkey for rod: Distance beetween start and end stoppers on rod (in mm)
            /// </summary>
            public const string SUBKEY_STOPPER_DIST = "_StopperDistance";

            /// <summary>
            /// Subkey for rod: Best effort position for current rod of Start stopper position (in mm)
            /// </summary>
            public const string SUBKEY_BEST_EFFORT = "_BestEffort";

            /// <summary>
            /// Subkey for rod: DC Encoder ticks between min and max possible positions of rod (in Ticks)
            /// </summary>
            public const string SUBKEY_TICKS = "_Ticks";

            /// <summary>
            /// Subkey for rod: Defines if Servo position will be updated based on Arduino feedback
            /// </summary>
            public const string SUBKEY_SERVO_FEEDBACK = "_ServoFeedback";

            /// <summary>
            /// Subkey for rod: Defines if Arduino Serial Port Per Rod
            /// </summary>
            public const string SUBKEY_COM_PORT = "_ArduinoPort";
        }
    }
}
