using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MiniJSON;
using UnityEngine;

public class Bedrock : MonoBehaviour
{
	public enum BedrockTextEncoding
	{
		ASCII_127 = 0,
		ASCII_255 = 1,
		UTF7 = 2,
		UTF8 = 3
	}

	public struct brUnityInitSettings
	{
		public string _titleName;

		public string _appVersion;

		public string _swrveId;

		public string _swrveKey;

		public string _swrveAnalyticsUrl;

		public string _swrveABUrl;

		public string _swrveOverrideUsername;

		public string _kochavaId;

		public string _logOnRewardMessage;

		public string _facebookAppId;

		public brRemoteNotificationType _remoteNotificationTypes;

		public int _anonymousLogonEnabled;

		public int _logonRegisteredEnabled;

		public int _pushNotificationsLocalEnabled;

		public int _pushNotificationsRemoteEnabled;

		public int _currencyInventoryEnabled;

		public ulong _swrveBatchUploadIntervalInSec;

		public brReportPriority _logLevel;

		public brEnvironment _environment;

		public brFriendMetaDataTypes _friendsListMetaDataSetting;

		public bool _enableSharedContentUsageSystemOnStartup;

		public string _resourceDirectory;

		public int _logToFileEnabled;

		public brLogDetailLevel _logDetailLevel;

		public int _gameCenterAuthenticationHandled;

		public int _swrveTalkEnabled;

		public int _noConnectionRetrys;

		public int _maxGameConnectionMessageType;

		public ushort _gamePort;

		public int _lanOnly;

		public string _swrveContentUrl;

		public string _swrveLinkUrl;
	}

	public enum brEnvironment
	{
		BR_ENV_DEVELOPMENT = 0,
		BR_ENV_CERTIFICATION = 1,
		BR_ENV_LIVE = 2,
		BR_ENV_MAX_ENVIRONMENTS = 3
	}

	public enum brReportPriority
	{
		BR_NOTICE = 0,
		BR_WARNING = 1,
		BR_ERROR = 2
	}

	public enum brResult
	{
		BR_SUCCESS = 0,
		BR_LIBRARY_NOT_INITIALIZED = 1,
		BR_USER_IS_NOT_LOGGED_IN = 2,
		BR_INVALID_PARAMETER = 3,
		BR_FEATURE_DISABLED = 4,
		BR_BUFFER_TOO_SMALL = 5,
		BR_INTERNAL_ERROR = 6,
		BR_REQUEST_ALREADY_PENDING = 7
	}

	public enum brLinkedAccountTypes
	{
		BR_ACCOUNT_TYPE_FACEBOOK = 1,
		BR_ACCOUNT_TYPE_XBL = 2,
		BR_ACCOUNT_TYPE_PSN = 4,
		BR_ACCOUNT_TYPE_NINTENDO = 8
	}

	public enum brTaskStatus
	{
		BR_TASK_NOT_IN_USE = 0,
		BR_TASK_INIT = 1,
		BR_TASK_PENDING = 2,
		BR_TASK_SUCCESS = 3,
		BR_TASK_FAIL = 4
	}

	public enum brLobbyServerTier
	{
		BR_FIRST_SERVER_TIER = 0,
		BR_LOBBY_SERVER_COMMON = 0,
		BR_LOBBY_SERVER_FRANCHISE = 1,
		BR_LOBBY_SERVER_TITLE = 2,
		BR_NUM_SERVER_TIERS = 3,
		BR_UNKNOWN_TIER = 3
	}

	public enum brSwrveTalkActionType
	{
		BR_SWRVE_ACTION_DISMISS = 0,
		BR_SWRVE_ACTION_CUSTOM = 1,
		BR_SWRVE_ACTION_INSTALL = 2,
		BR_SWRVE_ACTION_NONE = 3
	}

	public enum brLogDetailLevel
	{
		BR_LOG_DETAIL_PRIORITY_ONLY = 0,
		BR_LOG_DETAIL_CHANNEL = 2,
		BR_LOG_DETAIL_FUNCTION = 4,
		BR_LOG_DETAIL_FILE = 8,
		BR_LOG_DETAIL_LINE_NUMBER = 0x10
	}

	public enum brContentUnlockError
	{
		BR_CONTENT_UNLOCK_UNKNOWN_ERROR = 1300,
		BR_CONTENT_UNLOCK_KEY_INVALID = 1301,
		BR_CONTENT_UNLOCK_KEY_ALREADY_USED_UP = 1302,
		BR_CONTENT_UNLOCK_SHARED_UNLOCK_LIMIT_REACHED = 1303,
		BR_CONTENT_UNLOCK_DIFFERENT_HARDWARE_ID = 1304,
		BR_CONTENT_UNLOCK_INVALID_CONTENT_OWNER = 1305
	}

	public enum brCloudStorageFileConflictStatus
	{
		BR_CLOUDSTORAGE_FILECONFLICT_NONE = 0,
		BR_CLOUDSTORAGE_FILECONFLICT_ONLY_EXISTS_ON_CLOUD = 1,
		BR_CLOUDSTORAGE_FILECONFLICT_ONLY_EXISTS_ON_LOCAL = 2,
		BR_CLOUDSTORAGE_FILECONFLICT_CLOUD_IS_NEWER_SAME_DEVICE = 3,
		BR_CLOUDSTORAGE_FILECONFLICT_LOCAL_IS_NEWER_SAME_DEVICE = 4,
		BR_CLOUDSTORAGE_FILECONFLICT_CLOUD_IS_NEWER_DIFFERENT_DEVICE = 5,
		BR_CLOUDSTORAGE_FILECONFLICT_LOCAL_IS_NEWER_DIFFERENT_DEVICE = 6,
		BR_CLOUDSTORAGE_FILECONFLICT_LOCAL_CLOUD_DIFFERENT_FILES = 7,
		BR_CLOUDSTORAGE_FILECONFLICT_NO_FILE = 8
	}

	public enum brCloudStorageFileResolveAction
	{
		BR_CLOUDSTORAGE_RESOLVE_WITH_LOCAL_FILE = 0,
		BR_CLOUDSTORAGE_RESOLVE_WITH_CLOUD_FILE = 1
	}

	public enum brCloudStorageConnectionStatus
	{
		BR_CLOUDSTORAGE_CONNECTION_UNINITIALIZED = 0,
		BR_CLOUDSTORAGE_CONNECTION_OFFLINE = 1,
		BR_CLOUDSTORAGE_CONNECTION_ONLINE_PENDING = 2,
		BR_CLOUDSTORAGE_CONNECTION_ONLINE = 3
	}

	public enum brCloudStorageBehaviorFlags
	{
		BR_CLOUDSTORAGE_BEHAVIOR_DEFAULT = 0,
		BR_CLOUDSTORAGE_BEHAVIOR_FLAG_PEDANTIC_LOCALNEWER_CONFLICT = 1,
		BR_CLOUDSTORAGE_BEHAVIOR_FLAG_AUTOPUSH_CLOUD_UPDATES = 2,
		BR_CLOUDSTORAGE_BEHAVIOR_FLAG_ALLOW_ANONYMOUS_CLOUD_USAGE = 4
	}

	public enum brIAPReceiptBehavior
	{
		BR_IAP_RECEIPT_ANALYTICS_VALIDATION = 0,
		BR_IAP_RECEIPT_SKIP_VALIDATION = 1,
		BR_IAP_RECEIPT_APPLICATION_CONFIRMS_VALIDATION = 2
	}

	public enum brIAPProductCategory
	{
		BR_IAP_PRODUCT_CATEGORY_CONSUMABLE = 0,
		BR_IAP_PRODUCT_CATEGORY_UNLOCKABLE = 1,
		BR_IAP_PRODUCT_CATEGORY_SUBSCRIPTION = 2
	}

	public enum brIAPProductStatus
	{
		BR_IAP_PRODUCT_STATUS_UNKNOWN = 0,
		BR_IAP_PRODUCT_STATUS_NOT_VALID = 1,
		BR_IAP_PRODUCT_STATUS_PENDING_CATALOG_UPDATE = 2,
		BR_IAP_PRODUCT_STATUS_AVAILABLE = 3,
		BR_IAP_PRODUCT_STATUS_PURCHASE_PENDING = 4,
		BR_IAP_PRODUCT_STATUS_PURCHASE_CANCELED = 5,
		BR_IAP_PRODUCT_STATUS_PURCHASE_FAILED = 6,
		BR_IAP_PRODUCT_STATUS_PURCHASE_VALIDATION_FAILED = 7,
		BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATING = 8,
		BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATED = 9
	}

	public enum brIAPAvailabilityStatus
	{
		BR_IAP_AVAILABILITY_UNAVAILABLE = 0,
		BR_IAP_AVAILABILITY_PENDING_CATALOG_RETRIEVAL = 1,
		BR_IAP_AVAILABILITY_PURCHASES_DISABLED = 2,
		BR_IAP_AVAILABILITY_PURCHASES_ENABLED = 3,
		BR_IAP_AVAILABILITY_PURCHASES_ENABLED_NO_VERIFICATION = 4
	}

	[Flags]
	public enum brRemoteNotificationType
	{
		BR_NOTIFICATION_TYPE_NONE = 0,
		BR_NOTIFICATION_TYPE_BADGE = 1,
		BR_NOTIFICATION_TYPE_SOUND = 2,
		BR_NOTIFICATION_TYPE_ALERT = 4,
		BR_NOTIFICATION_TYPE_NEWSSTAND_CONTENT_AVAILABILITY = 8
	}

	public enum brEventType
	{
		BR_FULLY_CONNECTED = 0,
		BR_DISCONNECTED = 1,
		BR_LOG_ON_FAIL = 2,
		BR_PARAMETERS_AVAILABLE = 3,
		BR_USER_ABTEST_PARAMETERS_AVAILABLE = 4,
		BR_BACKGROUND_CONTENT_DOWNLOAD_COMPLETE = 5,
		BR_BACKGROUND_CONTENT_DOWNLOAD_FAILURE = 6,
		BR_BACKGROUND_CONTENT_UNLOCK_SYNC_COMPLETE = 7,
		BR_NEW_MESSAGE_AVAILABLE = 8,
		BR_USER_VARIABLES_CLOUD_CONFLICT = 9,
		BR_USER_VARIABLES_UPDATED_FROM_CLOUD = 10,
		BR_USER_VARIABLES_CLOUD_PUSH_SUCCEEDED = 11,
		BR_USER_VARIABLES_CLOUD_PUSH_FAILED = 12,
		BR_USER_VARIABLES_USER_CHANGED = 13,
		BR_IAP_CATALOG_REQUEST_COMPLETED = 14,
		BR_IAP_PURCHASE_REQUEST_COMPLETED = 15,
		BR_IAP_RECEIPT_VALIDATION_COMPLETED = 16,
		BR_FACEBOOK_AUTHORIZE_SUCCESS = 17,
		BR_FACEBOOK_AUTHORIZE_FAILURE = 18,
		BR_ACTIVATE_FACEBOOK_NEEDS_AUTHORIZATION = 19,
		BR_FACEBOOK_REQUEST_SUCCESS = 20,
		BR_FACEBOOK_REQUEST_FAILURE = 21,
		BR_USER_CONNECTION_STATUS_CHANGED = 22,
		BR_USERNAME_CHANGED = 23,
		BR_REGISTRATION_REWARD = 24,
		BR_OUTDATED = 25,
		BR_CLOUDSTORAGE_CONNECTED_TO_CLOUD = 26,
		BR_SHARED_CREDENTIALS_ACCEPTED = 27,
		BR_SHARED_CREDENTIALS_DENIED = 28,
		BR_FRIEND_CACHE_UPDATED = 29,
		BR_GAME_CENTER_AUTHENTICATION_SUCCESS = 30,
		BR_GAME_CENTER_AUTHENTICATION_FAILURE = 31,
		BR_GAME_CENTER_NEEDS_AUTHENTICATION = 32,
		BR_MAKE_GOOD_REWARD = 33,
		BR_SWRVE_TALK_MESSAGE_OPENED = 34,
		BR_SWRVE_TALK_MESSAGE_CLOSED = 35,
		BR_EMERGENCY_MESSAGE_AVAILABLE = 36,
		BR_EMERGENCY_MESSAGE_INVALID = 37,
		BR_GOOGLE_PLAY_AUTHORIZE_SUCCESS = 38,
		BR_GOOGLE_PLAY_AUTHORIZE_FAILURE = 39,
		BR_MULTIPLAYER_GAME_CONNECTION_ESTABLISHED = 40,
		BR_MULTIPLAYER_GAME_CONNECTION_DISCONNECTED = 41,
		BR_MULTIPLAYER_GAME_CONNECTION_FAILED = 42
	}

	[Flags]
	public enum brFriendMetaDataTypes
	{
		BR_NO_META_DATA = 0,
		BR_GAMES_OWNED = 2,
		BR_LAST_ONLINE = 4,
		BR_LAST_GAME_PLAYED = 8,
		BR_ALL_META_DATA = -1
	}

	public struct brCloudStorageParameters
	{
		public uint maxCloudFileCount;

		public brCloudStorageBehaviorFlags cloudStorageBehaviorFlags;
	}

	public struct brCloudStorageFileInfo
	{
		public ulong fileId;

		public uint fileSize;

		public uint localFileSize;

		public uint metadataSize;

		public uint metadataSizeLocalFile;

		public uint lastPushedTimestamp;

		public uint cloudLastPushedTimestamp;

		public brCloudStorageFileConflictStatus fileConflictStatus;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string lastPushedDeviceName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 120)]
		public string userMetadata;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 120)]
		public string userMetadataLocalFile;
	}

	public class IAPCatalogEntry
	{
		public brIAPProductCategory IAPProductCategory;

		public brIAPProductStatus IAPProductStatus;

		public ulong IAPProductVirtualCurrencyAmount;

		public float IAPProductRawPrice;

		public string IAPProductID;

		public string IAPLocalizedProductName;

		public string IAPLocalizedProductDescription;

		public string IAPLocalizedProductPrice;

		public string IAPProductCountryCode;

		public string IAPProductCurrencyCode;

		public string IAPProductVirtualCurrencyName;

		public IAPCatalogEntry()
		{
			IAPProductCategory = brIAPProductCategory.BR_IAP_PRODUCT_CATEGORY_CONSUMABLE;
			IAPProductStatus = brIAPProductStatus.BR_IAP_PRODUCT_STATUS_UNKNOWN;
			IAPProductVirtualCurrencyAmount = 0uL;
			IAPProductRawPrice = 0f;
			IAPProductID = string.Empty;
			IAPLocalizedProductName = string.Empty;
			IAPLocalizedProductDescription = string.Empty;
			IAPLocalizedProductPrice = string.Empty;
			IAPProductCountryCode = string.Empty;
			IAPProductCurrencyCode = string.Empty;
			IAPProductVirtualCurrencyName = string.Empty;
		}

		public IAPCatalogEntry(brIAPCatalogEntry catalogEntry)
		{
			IAPProductCategory = catalogEntry._IAPProductCategory;
			IAPProductStatus = catalogEntry._IAPProductStatus;
			IAPProductVirtualCurrencyAmount = catalogEntry._IAPProductVirtualCurrencyAmount;
			IAPProductRawPrice = catalogEntry._IAPProductRawPrice;
			IAPProductID = DecodeText(catalogEntry._IAPProductID);
			IAPLocalizedProductName = DecodeText(catalogEntry._IAPLocalizedProductName);
			IAPLocalizedProductDescription = DecodeText(catalogEntry._IAPLocalizedProductDescription);
			IAPLocalizedProductPrice = DecodeText(catalogEntry._IAPLocalizedProductPrice);
			IAPProductCountryCode = DecodeText(catalogEntry._IAPProductCountryCode);
			IAPProductCurrencyCode = DecodeText(catalogEntry._IAPProductCurrencyCode);
			IAPProductVirtualCurrencyName = DecodeText(catalogEntry._IAPProductVirtualCurrencyName);
		}
	}

	public struct brIAPCatalogEntry
	{
		public brIAPProductCategory _IAPProductCategory;

		public brIAPProductStatus _IAPProductStatus;

		public ulong _IAPProductVirtualCurrencyAmount;

		public float _IAPProductRawPrice;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _IAPProductID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] _IAPLocalizedProductName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] _IAPLocalizedProductDescription;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _IAPLocalizedProductPrice;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public byte[] _IAPProductCountryCode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public byte[] _IAPProductCurrencyCode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public byte[] _IAPProductVirtualCurrencyName;
	}

	public struct brKeyValuePair
	{
		public string key;

		public string val;
	}

	public struct brKeyValueArray
	{
		public int size;

		public brKeyValuePair[] pairs;
	}

	public enum brUserConnectionStatus
	{
		BR_LOGGED_OUT = 0,
		BR_LOGGING_IN_ANONYMOUSLY = 1,
		BR_LOGGING_IN_REGISTERED = 2,
		BR_LOGGED_IN_ANONYMOUSLY_ONLINE = 3,
		BR_LOGGED_IN_REGISTERED_ONLINE = 4,
		BR_LOGGED_IN_ANONYMOUSLY_OFFLINE = 5,
		BR_LOGGED_IN_REGISTERED_OFFLINE = 6
	}

	public enum brUserInterfaceReasonForClose
	{
		BR_UI_STILL_OPEN = 0,
		BR_UI_CLOSE_LOGIN = 1,
		BR_UI_CLOSE_LOGOUT = 2,
		BR_UI_CLOSE_CANCEL = 3,
		BR_UI_CLOSE_TERMINATE_LOGIN = 4,
		BR_UI_CLOSE_UNKNOWN = 5,
		BR_UI_CLOSE_INVALID_TOKEN = 6,
		BR_UI_CLOSE_MULTIPLE_LOG_ON = 7,
		BR_UI_CLOSE_AD_SHOWN_AND_ACCEPTED = 8,
		BR_UI_CLOSE_AD_NOT_SHOWN = 9,
		BR_UI_CLOSE_ACCOUNT_DELETED = 10,
		BR_UI_CLOSE_SHUTDOWN = 11,
		BR_UI_CLOSE_MAX = 12
	}

	public enum brUserInterfaceScreen
	{
		BR_UI_CLOSED = 0,
		BR_LOG_ON_UI = 1,
		BR_FRIENDS_UI = 2,
		BR_ADD_FRIENDS_UI = 3,
		BR_GAMES_LIST_UI = 4,
		BR_CUSTOMER_SERVICE_UI = 5,
		BR_CUSTOMER_SERVICE_UI_NO_LOGIN = 6,
		BR_UI_FACEBOOK_REAUTH = 7,
		BR_UI_ADVERTISEMENT = 8,
		BR_ACTIVATE_REGISTER_UI = 9,
		BR_FACEBOOK_REGISTER_UI = 10,
		BR_TERMS_OF_SERVICE_UI = 11,
		BR_TOY_LINK_UI = 12,
		BR_TOY_LINK_PROMO_UI = 13,
		BR_UI_MAX = 14
	}

	public enum brChallengeStatus
	{
		BR_CHALLENGE_UNKNOWN = 0,
		BR_CHALLENGE_ACTIVE = 1,
		BR_CHALLENGE_INACTIVE = 2,
		BR_CHALLENGE_INVALID = 3
	}

	public enum brBedrockGameId
	{
		BR_GAMEID_INVALID = 0,
		BR_GAMEID_BEDROCKEXAMPLE = 1,
		BR_GAMEID_CLOUDPATROL = 2,
		BR_GAMEID_PITFALL = 3,
		BR_GAMEID_KINGDOMS = 4,
		BR_GAMEID_WIPEOUT = 5,
		BR_GAMEID_CORONA = 6,
		BR_GAMEID_CLOUDPATROLKF = 7,
		BR_GAMEID_BATTLEGROUNDS = 8,
		BR_GAMEID_SWAP_FORCE = 9,
		BR_GAMEID_PITFALLANDROID = 10,
		BR_GAMEID_WIPEOUTANDROID = 12,
		BR_GAMEID_FACTIONS = 13,
		BR_GAMEID_COLLECTIONVAULT = 14,
		BR_GAMEID_CLOUDPATROLANDROID = 15,
		BR_GAMEID_BATTLEGROUNDSLGTV = 16,
		BR_GAMEID_CORONAANDROID = 17,
		BR_GAMEID_CORONAWP8 = 18,
		BR_GAMEID_BATTLEGROUNDSANDROID = 19,
		BR_GAMEID_KINGDOMSANDROID = 20,
		BR_GAMEID_KRAVE = 21,
		BR_GAMEID_KRAVEANDROID = 22,
		BR_GAMEID_KINGDOMSKF = 23,
		BR_GAMEID_BATTLEGROUNDSKF = 24,
		BR_GAMEID_COLLECTIONVAULTANDROID = 25,
		BR_GAMEID_TONYHAWK = 26,
		BR_GAMEID_TONYHAWKANDROID = 27,
		BR_GAMEID_CORONAKF = 28,
		BR_GAMEID_COLLECTIONVAULTKF = 32,
		BR_NUM_GAMEIDS = 33
	}

	public struct brChallengeInfo
	{
		public uint _challengeId;

		public uint _leaderboardId;

		public uint _startDate;

		public uint _activeDuration;

		public uint _inactiveDuration;

		public uint _invalidDuration;

		public uint _numResets;

		public brChallengeStatus _status;

		public bool _isParticipating;

		public IntPtr _rawData;
	}

	public enum brLeaderboardWriteType
	{
		BR_STAT_WRITE_REPLACE = 0,
		BR_STAT_WRITE_ADD = 1,
		BR_STAT_WRITE_MAX = 2,
		BR_STAT_WRITE_MIN = 3,
		BR_STAT_WRITE_REPLACE_WHEN_RATING_INCREASE = 4,
		BR_STAT_WRITE_ADD_WHEN_RATING_INCREASE = 5,
		BR_STAT_WRITE_MAX_WHEN_RATING_INCREASE = 6,
		BR_STAT_WRITE_MIN_WHEN_RATING_INCREASE = 7
	}

	public enum brFriendType
	{
		BR_FRIEND_TYPE_STRONG = 0,
		BR_FRIEND_TYPE_WEAK_FACEBOOK = 1,
		BR_FRIEND_TYPE_WEAK_GAMECENTER = 2,
		BR_FRIEND_TYPE_WEAK_GOOGLEPLUS = 3,
		BR_MAX_FRIEND_TYPES = 4
	}

	public struct brLeaderboardRow
	{
		public uint _leaderboardId;

		public ulong _userId;

		public brLeaderboardWriteType _writeType;

		public long _rating;

		public ulong _rank;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _entityName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		public int[] _integerFields;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		public float[] _floatFields;

		public uint _blobDataSize;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] _blobData;

		public bool isValid()
		{
			return _userId != 0;
		}

		public string getEntityName()
		{
			return DecodeText(_entityName);
		}
	}

	public struct brFacebookPostParameters
	{
		public string message;

		public string pictureURL;

		public string linkURL;

		public string linkName;

		public string linkCaption;

		public string linkDescription;

		public string sourceURL;

		public brFacebookPostParameters(string msg)
		{
			message = msg;
			pictureURL = string.Empty;
			linkURL = string.Empty;
			linkName = string.Empty;
			linkCaption = string.Empty;
			linkDescription = string.Empty;
			sourceURL = string.Empty;
		}

		public brFacebookPostParameters(string msg, string pictureUrl, string aLinkUrl, string alinkName, string aLinkCaption, string aLinkDescription, string sourceUrl)
		{
			message = msg;
			pictureURL = pictureUrl;
			linkURL = aLinkUrl;
			linkName = alinkName;
			linkCaption = aLinkCaption;
			linkDescription = aLinkDescription;
			sourceURL = sourceUrl;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct brFacebookReadPermissions
	{
	}

	public struct brFacebookPublishPermissions
	{
		public int publishActions;
	}

	public struct brFacebookActionParameters
	{
		public string actionNamespaceAndType;

		public string objectType;

		public string objectURL;

		public IntPtr customParameters;

		public uint numCustomParameters;

		public brFacebookActionParameters(string action, string objType, string url)
		{
			actionNamespaceAndType = action;
			objectType = objType;
			objectURL = url;
			customParameters = IntPtr.Zero;
			numCustomParameters = 0u;
		}
	}

	public class LogOnEventArgs : EventArgs
	{
		public bool AnonymousLogOn { get; private set; }

		public bool FirstTime { get; private set; }

		public LogOnEventArgs(bool anonymousLogOn, bool firstTime)
		{
			AnonymousLogOn = anonymousLogOn;
			FirstTime = firstTime;
		}
	}

	public class CloudConflictInfo : EventArgs
	{
		public brCloudStorageConnectionStatus connectionStatus;

		public brCloudStorageFileConflictStatus fileConflictStatus;

		public string metaData;

		public string deviceName;

		public CloudConflictInfo()
		{
			connectionStatus = brCloudStorageConnectionStatus.BR_CLOUDSTORAGE_CONNECTION_UNINITIALIZED;
			fileConflictStatus = brCloudStorageFileConflictStatus.BR_CLOUDSTORAGE_FILECONFLICT_NONE;
		}
	}

	public class RemoteNotificationEventArgs : EventArgs
	{
		public string _message;

		public RemoteNotificationEventArgs(string message)
		{
			_message = message;
		}
	}

	public struct brFriendInvitePayload
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 144)]
		public byte[] _message;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
		public byte[] _inviteSource;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
		public byte[] _inviteVersion;

		public byte _groupId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 843)]
		public byte[] _padding;
	}

	public struct brFriendInvite
	{
		public brFriendInvitePayload _payload;

		public ulong _userId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _senderName;
	}

	public struct brFriendInfo
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _displayName;

		public ulong _userId;

		public ulong _gamesOwnedSet0;

		public uint _lastOnlineTime;

		public ushort _lastGamePlayed;

		public byte _groupId;

		[MarshalAs(UnmanagedType.U1)]
		public bool _isOnline;

		public brFriendType _friendType;

		public override string ToString()
		{
			return string.Concat("{_displayName: ", _displayName, ",_userId:", _userId, ",_gamesOwnedSet0:", _gamesOwnedSet0, ",_lastOnlineTime:", _lastOnlineTime, ",_lastGamePlayed:", _lastGamePlayed, ",_groupId:", _groupId, ",_isOnline:", _isOnline, ",_friendType:", _friendType, "}");
		}
	}

	public struct brContentUnlockInfo
	{
		public uint contentKey;

		public uint subType;

		public void init()
		{
			contentKey = uint.MaxValue;
			subType = uint.MaxValue;
		}

		public override string ToString()
		{
			return string.Format("[brContentUnlockInfo contentKey={0},subType={1}]", contentKey, subType);
		}
	}

	public struct brLinkedAccountsInfo
	{
		public brLinkedFacebookAccountInfo _facebookAccountInfo;

		public brLinkedXBLAccountInfo _xblAccountInfo;

		public brLinkedPSNAccountInfo _psnAccountInfo;

		public brLinkedNintendoAccountInfo _nintendoAccountInfo;
	}

	public struct brLinkedFacebookAccountInfo
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _accountID;
	}

	public struct brLinkedXBLAccountInfo
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _accountID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _username;
	}

	public struct brLinkedPSNAccountInfo
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _accountID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _username;
	}

	public struct brLinkedNintendoAccountInfo
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _accountID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] _username;
	}

	public class MakeGoodRewardInfo
	{
		public int RewardType { get; private set; }

		public int Value { get; private set; }

		public uint TimeStamp { get; private set; }

		public MakeGoodRewardInfo(brMakeGoodRewardInfo rewardInfo)
		{
			RewardType = rewardInfo.type;
			Value = rewardInfo.value;
			TimeStamp = rewardInfo.timestamp;
		}

		public override string ToString()
		{
			return string.Format("[MakeGoodRewardInfo: RewardType={0}, Value={1}, TimeStamp={2}]", RewardType, Value, TimeStamp);
		}
	}

	public struct brMakeGoodRewardInfo
	{
		public int type;

		public int value;

		public uint timestamp;
	}

	public class BedrockAndroidJavaBridge
	{
		private static AndroidJavaObject BedrockInterface_plugin;

		private static AndroidJavaObject BedrockWrapper_plugin;

		private static bool _bridgeStarted;

		static BedrockAndroidJavaBridge()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				Debug.Log("JavaBridge: creating BedrockWrapper");
				BedrockWrapper_plugin = new AndroidJavaObject("com.vvisions.bedrock.wrapper.BedrockWrapper", @static);
				Debug.Log("JavaBridge: creating BedrockInterface");
				BedrockInterface_plugin = new AndroidJavaObject("com.vvisions.bedrock.BedrockInterface", @static, BedrockWrapper_plugin);
				string androidGcmProjectID = Instance._androidGcmProjectID;
				string androidIapPublicKey = Instance._androidIapPublicKey;
				Debug.Log("JavaBridge: setting BedrockInterface on wrapper.");
				BedrockWrapper_plugin.Call("setBedrockInterface", BedrockInterface_plugin, androidGcmProjectID, androidIapPublicKey);
				BedrockWrapper_plugin.Call("onStart");
				Debug.Log("JavaBridge: setting BedrockInterface on activity override.");
				@static.Call("setBedrockInterface", BedrockInterface_plugin);
				Debug.Log("JavaBridge: finished setting BedrockInterface on activity override.");
				_bridgeStarted = false;
			}
		}

		public static void StartJavaBridge()
		{
			_bridgeStarted = true;
		}

		public static void RestartJavaBridge()
		{
			//BedrockInterface_plugin.Call("resetBedrockInterface");
		}

		public static bool GetBridgePreviouslyStarted()
		{
			return _bridgeStarted;
		}
	}

	public class brUserInterfaceReasonForCloseEventArgs : EventArgs
	{
		private brUserInterfaceReasonForClose _reason;

		public brUserInterfaceReasonForClose Reason
		{
			get
			{
				return _reason;
			}
		}

		public brUserInterfaceReasonForCloseEventArgs(brUserInterfaceReasonForClose reason)
		{
			_reason = reason;
		}
	}

	private const string ImportLib = "bedrock";

	public const short InvalidTaskHandle = -1;

	public const int BR_MAX_USERNAME_LENGTH = 64;

	public const int BR_MAX_SWRVE_TALK_CUSTOM_ACTION_LENGTH = 128;

	public const int BR_MAX_DEVICE_NAME_LENGTH = 64;

	public const int BR_MAX_CLOUD_FILE_USER_METADATA_SIZE = 120;

	public const int BR_MAX_INVITE_MESSAGE_LENGTH = 144;

	public const int BR_MAX_INVITE_SOURCE_LENGTH = 24;

	public const int BR_MAX_INVITE_VERSION_LENGTH = 12;

	public const int BR_MAX_IAP_PRODUCT_ID_LENGTH = 64;

	public const int BR_MAX_IAP_PRODUCT_NAME_LENGTH = 128;

	public const int BR_MAX_IAP_PRODUCT_DESCRIPTION_LENGTH = 256;

	public const int BR_MAX_IAP_PRODUCT_PRICE_LENGTH = 64;

	public const int BR_MAX_IAP_PRODUCT_LOCALE_INFO_LENGTH = 32;

	public const int BR_MAX_IAP_PRODUCT_VIRTUAL_CURRENCY_NAME_LENGTH = 32;

	public const int BR_MAX_FRIENDS = 100;

	public const int BR_MAX_FILENAME_LENGTH = 240;

	public const int BR_MAX_FILEPATH_LENGTH = 260;

	public const int BR_MAX_MANIFEST_FILEID_LENGTH = 240;

	public const int BR_MAX_ACCOUNT_INFO_LENGTH = 64;

	public const int BR_MAX_LEADERBOARD_BLOB_SIZE = 128;

	public const int BR_LEADERBOARD_ROW_NUM_FIELDS = 5;

	public BedrockPlatformConfiguration applePlatformConfiguration;

	public BedrockPlatformConfiguration androidPlatformConfiguration;

	public BedrockPlatformConfiguration kindlePlatformConfiguration;

	public float _UpdateIntervalSeconds = 0.1f;

	public bool _anonymousLogonEnabled = true;

	public bool _registeredLogonEnabled = true;

	public bool _localPushNotificationsEnabled = true;

	public bool _remotePushNotificationsEnabled = true;

	public bool _currencyInventoryEnabled;

	[HideInInspector]
	public brRemoteNotificationType _remoteNotificationTypes = brRemoteNotificationType.BR_NOTIFICATION_TYPE_SOUND | brRemoteNotificationType.BR_NOTIFICATION_TYPE_ALERT;

	public string _logOnRewardMessage;

	public string _millenialMediaGoalId;

	public string _facebookAppId;

	public brReportPriority _logLevel;

	public bool _useProductionServers;

	public bool _useDebugSwrveAPI;

	public bool _autoDownloadUserResources;

	public int _swrveBatchUploadIntervalInSeconds;

	public string _swrveOverrideUsername = string.Empty;

	public string _sandboxSwrveAnalyticsUrl = "https://activision.api.swrve.com";

	public string _sandboxSwrveABUrl = "https://activision.abtest.swrve.com";

	public string _debugSwrveAnalyticsUrl = "https://activision-debug.api.swrve.com";

	public string _productionSwrveAnalyticsUrl = "https://activision.api.swrve.com";

	public string _productionSwrveABUrl = "https://activision.abtest.swrve.com";

	public bool _swrveTalkEnabled;

	public string _swrveContentUrl = "http://activision.content.swrve.com";

	public string _swrveLinkUrl = "https://activision.link.swrve.com";

	public string _facebookJoinMeRequestTitle = "UNINITIALIZED_FACEBOOK_JOIN_ME_REQUEST_TITLE";

	public string _facebookJoinMeRequestMessage = "UNINITIALIZED_FACEBOOK_JOIN_ME_REQUEST_MESSAGE";

	public BedrockTextEncoding _textEncoding = BedrockTextEncoding.UTF8;

	public bool _bedrockInitializedExternally;

	public int _swrveMinimumAutomaticUpdateIntervalInMinutes = 60;

	public brFriendMetaDataTypes _friendsListMetaDataSetting;

	public bool _enableSharedContentUsageSystemOnStartup;

	public string _androidGcmProjectID = string.Empty;

	public string _androidIapPublicKey = string.Empty;

	private uint _maxCloudStorageFiles = 10u;

	private float _timeSinceLastUpdate;

	private bool _hasConnected;

	private TimeSpan _serverTimeOffset = TimeSpan.MaxValue;

	private DateTime _lastServerTimeUpdate = DateTime.MinValue;

	private DateTime _lastSwrveUpdateTime = DateTime.MinValue;

	private ulong _lastSwrveUserId;

	public static bool _UsePlayerPrefsOnUnsupportedPlatforms = true;

	private static bool _bedrockActive;

	private static Bedrock _instance;

	private static brUserConnectionStatus _connectionStatus;

	public TimeSpan ServerTimeOffset
	{
		get
		{
			return _serverTimeOffset;
		}
	}

	public DateTime LastServerTimeUpdate
	{
		get
		{
			return _lastServerTimeUpdate;
		}
	}

	public static Bedrock Instance
	{
		get
		{
			return _instance;
		}
	}

	private BedrockPlatformConfiguration CurrentPlatformConfiguration
	{
		get
		{
			return androidPlatformConfiguration;
		}
	}

	public static bool FacebookEnabled
	{
		get
		{
			bool result = false;
			if (isBedrockActive() && brIsFacebookEnabled(out result) != 0)
			{
				result = false;
			}
			return result;
		}
		set
		{
			if (isBedrockActive() && brEnableFacebook(value) != 0)
			{
				Debug.LogError("Unable to enable Facebook");
			}
		}
	}

	public static bool UseMainelineWebsite
	{
		get
		{
			if (isBedrockActive())
			{
				return brGetUseMainlineWebsite();
			}
			return false;
		}
		set
		{
			if (isBedrockActive())
			{
				brResult taskResult = brSetUseMainlineWebsite(value);
				CheckTaskResult(taskResult);
			}
		}
	}

	public static event EventHandler<EventArgs> TitleParametersChanged;

	public static event EventHandler<EventArgs> UnlockContentChanged;

	public static event EventHandler<EventArgs> UserResourcesChanged;

	public static event EventHandler<LogOnEventArgs> UserLogOn;

	public static event EventHandler<EventArgs> UserLogOnFail;

	public static event EventHandler<EventArgs> UserLogOff;

	public static event EventHandler<EventArgs> UserVarCloudConflict;

	public static event EventHandler<EventArgs> UserVarUserChanged;

	public static event EventHandler<EventArgs> UserVarUpdatedFromCloud;

	public static event EventHandler<EventArgs> UserVarCloudPushSucceeded;

	public static event EventHandler<EventArgs> UserVarCloudPushFailed;

	public static event EventHandler<EventArgs> UserConnectionStatusChanged;

	public static event EventHandler<EventArgs> UsernameChanged;

	public static event EventHandler<EventArgs> CloudStorageConnected;

	public static event EventHandler<EventArgs> IAPCatalogRetrieved;

	public static event EventHandler<EventArgs> IAPRequestCompleted;

	public static event EventHandler<RemoteNotificationEventArgs> RemoteNotificationReceived;

	public static event EventHandler<EventArgs> FriendCacheUpdated;

	public static event EventHandler<EventArgs> SharedCredentialsAccepted;

	public static event EventHandler<EventArgs> SharedCredentialsDenied;

	public static event EventHandler<EventArgs> MakeGoodRewardAvailable;

	public static event EventHandler<EventArgs> SwrveTalkMessageOpened;

	public static event EventHandler<EventArgs> SwrveTalkMessageClosed;

	public static event EventHandler<EventArgs> EmergencyMessageAvailable;

	public static event EventHandler<EventArgs> EmergencyMessageInvalid;

	public static event EventHandler<EventArgs> FacebookNeedsAuthorization;

	public static event EventHandler<EventArgs> FacebookRequestSuccess;

	public static event EventHandler<EventArgs> FacebookRequestFailure;

	public static event EventHandler<EventArgs> FacebookAuthorizeSuccess;

	public static event EventHandler<EventArgs> FacebookAuthorizeFailure;

	public static event EventHandler<brUserInterfaceReasonForCloseEventArgs> BedrockUIClosed;


	[DllImport("bedrock")]
	private static extern brResult brDisplayUserInterfaceUnity(int uiScreen);

	[DllImport("bedrock")]
	private static extern brResult brShutdown();

	[DllImport("bedrock")]
	private static extern brResult brUpdate();

	[DllImport("bedrock")]
	private static extern brResult brGetVersionString(byte[] outputBuffer, uint bufferSize);

	[DllImport("bedrock")]
	private static extern uint brGetVersion();

	[DllImport("bedrock")]
	private static extern brResult brGetDeviceName(byte[] outputBuffer, uint bufferSize);

	[DllImport("bedrock")]
	private static extern brResult brGetSwrveTalkMessageResult(ref brSwrveTalkActionType actionType, byte[] customActionBuffer, ref int customActionBufferSize, ref int game);

	[DllImport("bedrock")]
	private static extern brResult brClearSwrveTalkMessageResult();

	[DllImport("bedrock")]
	private static extern brResult brAnalyticsLogEvent(string name, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize, bool trackTime);

	[DllImport("bedrock")]
	private static extern brResult brAnalyticsEndTimedEvent(string name);

	[DllImport("bedrock")]
	private static extern brResult brAnalyticsSetCustomUserInformation([MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("bedrock")]
	private static extern brResult brAnalyticsLogVirtualPurchase(string itemID, ulong itemCost, ulong quantity, string virtualCurrencyType, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("bedrock")]
	private static extern brResult brAnalyticsLogRealPurchase(ulong realCost, string localCurrencyCode, string paymentProvider, ulong virtualCurrencyAmount, string virtualCurrencyType, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("bedrock")]
	private static extern brResult brAnalyticsLogRealPurchaseAsFloat(float realCost, string localCurrencyCode, string paymentProvider, ulong virtualCurrencyAmount, string virtualCurrencyType, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("bedrock")]
	private static extern brResult brAnalyticsLogVirtualCurrencyAwarded(ulong virtualCurrencyAmount, string virtualCurrencyType, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("bedrock")]
	private static extern brResult brGetFirstLogOnRewardEarned(out bool earned);

	[DllImport("bedrock")]
	private static extern brResult brReconnect();

	[DllImport("bedrock")]
	private static extern brResult brGetRemoteVariableAsString(string key, byte[] byteBuffer, ref uint byteBufferSize);

	[DllImport("bedrock")]
	private static extern brResult brGetRemoteVariableAsInt(string key, out int val);

	[DllImport("bedrock")]
	private static extern brResult brSetUserCacheVariableAsFloat(string key, float val);

	[DllImport("bedrock")]
	private static extern brResult brGetRemoteVariableAsFloat(string key, out float val);

	[DllImport("bedrock")]
	private static extern brResult brSetUserCacheVariableAsString(string key, string val);

	[DllImport("bedrock")]
	private static extern brResult brSetUserCacheVariableAsInt(string key, int val);

	[DllImport("bedrock")]
	private static extern brResult brGetUserCacheVariableAsFloat(string key, out float val);

	[DllImport("bedrock")]
	private static extern brResult brGetUserCacheVariableAsString(string key, byte[] byteBuffer, ref uint byteBufferSize);

	[DllImport("bedrock")]
	private static extern brResult brGetUserCacheVariableAsInt(string key, out int val);

	[DllImport("bedrock")]
	private static extern brResult brDeleteUserCacheVariable(string key);

	[DllImport("bedrock")]
	private static extern brResult brDeleteAllUserCacheVariables();

	[DllImport("bedrock")]
	private static extern bool brHasUserCacheVariable(string key);

	[DllImport("bedrock")]
	private static extern brResult brMoveAnonymousUserCacheDataToUser();

	[DllImport("bedrock")]
	private static extern brResult brGetUserCacheVariablesCloudConflictInfo(ref brCloudStorageConnectionStatus connectionStatus, ref brCloudStorageFileConflictStatus conflictStatus, byte[] cloudMetadataStringBuffer, ref uint cloudMetadataStringBufferSize, byte[] deviceNameStringBuffer, ref uint deviceNameStringBufferSize);

	[DllImport("bedrock")]
	private static extern brResult brResolveUserCacheVariablesWithCloud(brCloudStorageFileResolveAction resolveAction);

	[DllImport("bedrock")]
	private static extern brResult brGetUserCacheVariablesCloudFileInformationUnity(ref brCloudStorageFileInfo cloudStorageFileInfo);

	[DllImport("bedrock")]
	private static extern bool brGetConnected();

	[DllImport("bedrock")]
	private static extern brResult brGetUsername(byte[] byteBuffer, uint bufferSize);

	[DllImport("bedrock")]
	private static extern brUserConnectionStatus brGetUserConnectionStatus();

	[DllImport("bedrock")]
	private static extern brResult brBeginAsyncRetrieveUserResources();

	[DllImport("bedrock")]
	private static extern brResult brGetRemoteUserResourcesAsJSON(string itemId, byte[] byteBuffer, ref uint byteBufferSize);

	[DllImport("bedrock")]
	private static extern brResult brSetCloudStorageParameters(ref brCloudStorageParameters cloudStorageParameters);

	[DllImport("bedrock")]
	private static extern brIAPAvailabilityStatus brGetInAppPurchasingAvailability();

	[DllImport("bedrock")]
	private static extern brResult brSetInAppPurchasingReceiptVerificationBehavior(brIAPReceiptBehavior receiptBehavior);

	[DllImport("bedrock")]
	private static extern brResult brInitializeInAppPurchasingCatalog([MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] string[] productIDArray, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brIAPProductCategory[] productCategoryArray, uint catalogItemCount);

	[DllImport("bedrock")]
	private static extern brResult brSetInAppPurchaseCatalogEntryVirtualCurrencyInfo(string productId, string virtualCurrencyName, ulong virtualCurrencyAmount);

	[DllImport("bedrock")]
	private static extern brResult brGetInAppPurchasingCatalogEntryData(string productId, ref brIAPCatalogEntry catalogEntry);

	[DllImport("bedrock")]
	private static extern short brRequestInAppPurchase(string productId);

	[DllImport("bedrock")]
	private static extern short brValidateLastInAppPurchaseReceipt();

	[DllImport("bedrock")]
	private static extern brResult brGetInAppPurchasingStoredCompletedPurchaseCount(out uint numCompletedPurchases);

	[DllImport("bedrock")]
	private static extern brResult brGetInAppPurchasingFirstCompletedStoredPurchase(ref brIAPCatalogEntry catalogEntry);

	[DllImport("bedrock")]
	private static extern brResult brClearInAppPurchasingFirstCompletedStoredPurchase();

	[DllImport("bedrock")]
	private static extern ulong brGetDefaultOnlineId();

	[DllImport("bedrock")]
	private static extern brResult brDeviceAnonymousLogOn();

	[DllImport("bedrock")]
	private static extern bool brGetLoggedOnAnonymously();

	[DllImport("bedrock")]
	private static extern short brUnlockContent(brLobbyServerTier level, string licenseCode);

	[DllImport("bedrock")]
	private static extern brResult brListUnlockedContent(brLobbyServerTier level, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brContentUnlockInfo[] contentKeyBuffer, uint contentKeyBufferSize);

	[DllImport("bedrock")]
	private static extern short brSynchronizeUnlockedContentWithDeviceAnonymousAccount(brLobbyServerTier level);

	[DllImport("bedrock")]
	private static extern void brUpdateUnlockedContentCache(brLobbyServerTier level);

	[DllImport("bedrock")]
	private static extern short brQueryContentFromLicense(brLobbyServerTier level, string licenseCode);

	[DllImport("bedrock")]
	private static extern brResult brReadContentKeyFromLicenseQuery(short taskHandle, ref brContentUnlockInfo contentKey);

	[DllImport("bedrock")]
	private static extern brResult brStartTask(short taskHandle);

	[DllImport("bedrock")]
	private static extern brResult brEndTask(ref short taskHandle);

	[DllImport("bedrock")]
	private static extern brResult brGetTaskStatus(short taskHandle, out brTaskStatus status);

	[DllImport("bedrock")]
	private static extern brResult brGetTaskErrorCode(short taskHandle, ref int errorCode);

	[DllImport("bedrock")]
	private static extern short brGetServerTime(brLobbyServerTier level, IntPtr notUsed);

	[DllImport("bedrock")]
	private static extern brResult brGetServerTimeTaskResult(short taskHandle, out uint time);

	[DllImport("bedrock")]
	private static extern short brSendChallengeNotification([MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 1)] ulong[] targetIds, int numTargets, uint challengeId, string challengeDesc, int badge, string sound, byte[] payload);

	[DllImport("bedrock")]
	private static extern brResult brGetNumChallenges(out uint numChallenges);

	[DllImport("bedrock")]
	private static extern short brGetChallengeStatus(uint challengeId, IntPtr unused);

	[DllImport("bedrock")]
	private static extern brResult brGetChallengeStatusResult(short taskHandle, out brChallengeStatus status);

	[DllImport("bedrock")]
	private static extern short brUpdateStatusOfAllChallenges(IntPtr unused);

	[DllImport("bedrock")]
	private static extern brResult brCopyChallengeInfo(uint challengeId, out brChallengeInfo challengeInfo);

	[DllImport("bedrock")]
	private static extern short brWriteLeaderboardRow(brLobbyServerTier level, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brLeaderboardRow[] rows, int numRows);

	[DllImport("bedrock")]
	private static extern short brReadLeaderboardByPivot(brLobbyServerTier level, uint leaderboardId, IntPtr unused, uint maxResults, bool friendsOnly);

	[DllImport("bedrock")]
	private static extern short brReadLeaderboardByGivenPivot(brLobbyServerTier level, uint leaderboardId, ulong pivotUserId, IntPtr unused, uint maxResults, bool friendsOnly);

	[DllImport("bedrock")]
	private static extern short brReadLeaderboardByRank(brLobbyServerTier level, uint leaderboardId, ulong rank, IntPtr unused, uint maxResults, bool friendsOnly);

	[DllImport("bedrock")]
	private static extern short brReadLeaderboardByRating(brLobbyServerTier level, uint leaderboardId, ulong rating, IntPtr unused, uint maxResults, bool friendsOnly);

	[DllImport("bedrock")]
	private static extern short brReadLeaderboardByUserIds(brLobbyServerTier level, uint leaderboardId, ulong[] userIds, IntPtr unused, uint maxResults, bool friendsOnly);

	[DllImport("bedrock")]
	private static extern brResult brLeaderboardGetResultsFromTask(short task, [In][Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brLeaderboardRow[] rows, uint maxResults, bool friendsOnly);

	[DllImport("bedrock")]
	private static extern brResult brFacebookPostToWall(ref brFacebookPostParameters post);

	[DllImport("bedrock")]
	private static extern brResult brFacebookPostAction(ref brFacebookActionParameters action);

	[DllImport("bedrock")]
	private static extern brResult brFacebookRequestReadPermissions(ref brFacebookReadPermissions permissions);

	[DllImport("bedrock")]
	private static extern brResult brFacebookRequestPublishPermissions(ref brFacebookPublishPermissions permissions);

	[DllImport("bedrock")]
	private static extern brResult brEnableFacebook(bool enabled);

	[DllImport("bedrock")]
	private static extern brResult brIsFacebookEnabled(out bool enabled);

	[DllImport("bedrock")]
	private static extern brResult brFacebookInitAppRequest(string title, string message);

	[DllImport("bedrock")]
	private static extern brResult brUpdateFriendsList();

	[DllImport("bedrock")]
	private static extern short brGetIncomingFriendInvites(IntPtr unused, IntPtr unused2, uint maxInvites);

	[DllImport("bedrock")]
	private static extern brResult brGetIncomingFriendInvitesFromTask(short taskHandle, [In][Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brFriendInvite[] incomingInvites, out uint numInvitesReceived, uint maxInvites);

	[DllImport("bedrock")]
	private static extern brResult brGetNumIncomingFriendInvitesFromTask(short taskHandle, bool ignoreAutoInvites, out uint numInvitesReceived);

	[DllImport("bedrock")]
	private static extern brResult brGetFriendsWithCurrentGame([In][Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brFriendInfo[] friendsWithGameList, uint friendsWithGameBufferSize, ref uint numFriendsWithGame);

	[DllImport("bedrock")]
	private static extern short brGetLinkedAccounts(uint accountTypes);

	[DllImport("bedrock")]
	private static extern brResult brGetLinkedAccountsInfo(short taskHandle, ref brLinkedAccountsInfo linkedAccountsInfo);

	[DllImport("bedrock")]
	private static extern brResult brCheckSharedCredentials();

	[DllImport("bedrock")]
	private static extern bool brGetApplicationInstalled(brBedrockGameId application);

	[DllImport("bedrock")]
	private static extern brResult brGetMakeGoodReward(ref brMakeGoodRewardInfo reward);

	[DllImport("bedrock")]
	private static extern brResult brClearMakeGoodReward();

	[DllImport("bedrock")]
	private static extern short brGetSupportCode();

	[DllImport("bedrock", CharSet = CharSet.Auto)]
	private static extern brResult brGetSupportCodeTaskResult(short taskHandle, StringBuilder supportCodeBuffer, uint supportCodeBufferSize);

	[DllImport("bedrock")]
	private static extern short brUpdateSharedContentUsageForUser(brBedrockGameId gameId, int contentType, int contentSubType, uint serverTimeStamp, IntPtr unusedData, uint unusedDataSize);

	[DllImport("bedrock", CharSet = CharSet.Auto)]
	private static extern brResult brGetEmergencyMessage(string languageCode, StringBuilder messageBuffer, ref uint messageBufferSize);

	[DllImport("bedrock")]
	private static extern brResult brHasEmergencyMessage(out bool hasMessage);

	[DllImport("bedrock")]
	private static extern brResult brCheckForNewEmergencyMessage();

	[DllImport("bedrock")]
	private static extern brResult brWebsiteGetSubDir(byte[] byteBuffer, ref uint byteBufferSize);

	[DllImport("bedrock")]
	private static extern brResult brWebsiteSetSubDir(string subDir);

	[DllImport("bedrock")]
	private static extern brResult brSetUseMainlineWebsite(bool isEnabled);

	[DllImport("bedrock")]
	private static extern bool brGetUseMainlineWebsite();

	private static bool isSupportedPlatform()
	{
		return false; // Application.platform == RuntimePlatform.WP8Player || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
	}

	public static bool isBedrockActive()
	{
		/*if (isSupportedPlatform() && !_bedrockActive)
		{
			Debug.LogError("Bedrock was called before it was activated.");
		}*/
		return isSupportedPlatform() && _bedrockActive;
	}

	public static string DecodeText(byte[] rawBytes)
	{
		string text = string.Empty;
		if ((bool)_instance)
		{
			switch (_instance._textEncoding)
			{
			case BedrockTextEncoding.ASCII_127:
			{
				StringBuilder stringBuilder2 = new StringBuilder(rawBytes.Length);
				for (int j = 0; j < rawBytes.Length; j++)
				{
					if (rawBytes[j] < 128)
					{
						char value2 = (char)rawBytes[j];
						stringBuilder2.Append(value2);
					}
				}
				text = stringBuilder2.ToString();
				break;
			}
			case BedrockTextEncoding.ASCII_255:
			{
				StringBuilder stringBuilder = new StringBuilder(rawBytes.Length);
				for (int i = 0; i < rawBytes.Length; i++)
				{
					char value = (char)rawBytes[i];
					stringBuilder.Append(value);
				}
				text = stringBuilder.ToString();
				break;
			}
			case BedrockTextEncoding.UTF7:
				text = Encoding.UTF7.GetString(rawBytes);
				break;
			case BedrockTextEncoding.UTF8:
				text = Encoding.UTF8.GetString(rawBytes);
				break;
			}
		}
		else
		{
			text = Encoding.UTF8.GetString(rawBytes);
		}
		int num = text.IndexOf('\0');
		if (num >= 0)
		{
			text = text.Remove(num);
		}
		return text;
	}

	public void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			_bedrockActive = true;
			UnityEngine.Object.DontDestroyOnLoad(this);
			if (!BedrockAndroidJavaBridge.GetBridgePreviouslyStarted())
			{
				BedrockAndroidJavaBridge.StartJavaBridge();
			}
			if (!_instance._bedrockInitializedExternally)
			{
				initializeBedrock();
				brCloudStorageParameters cloudStorageParameters = default(brCloudStorageParameters);
				cloudStorageParameters.maxCloudFileCount = _maxCloudStorageFiles;
				cloudStorageParameters.cloudStorageBehaviorFlags = brCloudStorageBehaviorFlags.BR_CLOUDSTORAGE_BEHAVIOR_FLAG_AUTOPUSH_CLOUD_UPDATES;
				if (!SetCloudStorageParameters(ref cloudStorageParameters))
				{
					Debug.Log("SetCloudStorageParameters Failed.");
				}
				SetInAppPurchasingReceiptVerificationBehavior(brIAPReceiptBehavior.BR_IAP_RECEIPT_APPLICATION_CONFIRMS_VALIDATION);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public static void StartUp()
	{
		if (_instance != null && !_bedrockActive)
		{
			_instance._hasConnected = false;
			_bedrockActive = true;
			_instance.initializeBedrock();
		}
	}

	private void initializeBedrock()
	{
		if (!isBedrockActive())
		{
			return;
		}
		brUnityInitSettings settings = default(brUnityInitSettings);
		settings._titleName = CurrentPlatformConfiguration._titleName;
		settings._appVersion = CurrentPlatformConfiguration._appVersion;
		settings._anonymousLogonEnabled = (_anonymousLogonEnabled ? 1 : 0);
		settings._logonRegisteredEnabled = (_registeredLogonEnabled ? 1 : 0);
		settings._logLevel = _logLevel;
		settings._remoteNotificationTypes = _remoteNotificationTypes;
		settings._kochavaId = CurrentPlatformConfiguration._kochavaAppId;
		settings._logOnRewardMessage = _logOnRewardMessage;
		settings._facebookAppId = _facebookAppId;
		settings._swrveOverrideUsername = _swrveOverrideUsername;
		settings._pushNotificationsLocalEnabled = (_localPushNotificationsEnabled ? 1 : 0);
		settings._pushNotificationsRemoteEnabled = (_remotePushNotificationsEnabled ? 1 : 0);
		settings._currencyInventoryEnabled = (_currencyInventoryEnabled ? 1 : 0);
		settings._swrveBatchUploadIntervalInSec = (ulong)_swrveBatchUploadIntervalInSeconds;
		settings._friendsListMetaDataSetting = _friendsListMetaDataSetting;
		settings._enableSharedContentUsageSystemOnStartup = _enableSharedContentUsageSystemOnStartup;
		_useProductionServers = true;
		if (_useProductionServers)
		{
			settings._environment = brEnvironment.BR_ENV_LIVE;
			settings._swrveId = CurrentPlatformConfiguration._productionSwrveId;
			settings._swrveKey = CurrentPlatformConfiguration._productionSwrveKey;
			settings._swrveAnalyticsUrl = _productionSwrveAnalyticsUrl;
			settings._swrveABUrl = _productionSwrveABUrl;
		}
		else
		{
			settings._environment = brEnvironment.BR_ENV_DEVELOPMENT;
			settings._swrveId = CurrentPlatformConfiguration._sandboxSwrveId;
			settings._swrveKey = CurrentPlatformConfiguration._sandboxSwrveKey;
			if (_useDebugSwrveAPI)
			{
				settings._swrveAnalyticsUrl = _debugSwrveAnalyticsUrl;
			}
			else
			{
				settings._swrveAnalyticsUrl = _sandboxSwrveAnalyticsUrl;
			}
			settings._swrveABUrl = _sandboxSwrveABUrl;
		}
		settings._swrveTalkEnabled = (_swrveTalkEnabled ? 1 : 0);
		settings._swrveContentUrl = _swrveContentUrl;
		settings._swrveLinkUrl = _swrveLinkUrl;
		if (BedrockAndroidJavaBridge.GetBridgePreviouslyStarted())
		{
			BedrockAndroidJavaBridge.RestartJavaBridge();
		}
		//initBedrock(ref settings);
		//brFacebookInitAppRequest(Language.Get(_facebookJoinMeRequestTitle), Language.Get(_facebookJoinMeRequestMessage));
	}

	private void Update()
	{
		if (isBedrockActive() && Time.realtimeSinceStartup - _timeSinceLastUpdate >= _UpdateIntervalSeconds)
		{
			_timeSinceLastUpdate = Time.realtimeSinceStartup;
			brUpdate();
		}
	}

	public static void ForceUpdate()
	{
		if (isBedrockActive() && _instance != null)
		{
			_instance.Update();
		}
	}

	public static bool Shutdown()
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive() && _instance != null)
		{
			taskResult = brShutdown();
			_bedrockActive = false;
		}
		return CheckTaskResult(taskResult);
	}

	public static bool hasConnectedAfterStartup()
	{
		bool result = false;
		if (_instance != null)
		{
			result = _instance._hasConnected;
		}
		return result;
	}

	public static string GetBedrockVersionString()
	{
		string result = string.Empty;
		if (isBedrockActive())
		{
			byte[] array = new byte[128];
			if (brGetVersionString(array, 128u) == brResult.BR_SUCCESS)
			{
				result = DecodeText(array);
			}
		}
		return result;
	}

	public static uint GetBedrockVersion()
	{
		if (isBedrockActive())
		{
			return brGetVersion();
		}
		return 0u;
	}

	public static string GetDeviceName()
	{
		string result = string.Empty;
		if (isBedrockActive())
		{
			byte[] array = new byte[128];
			if (brGetDeviceName(array, 128u) == brResult.BR_SUCCESS)
			{
				result = DecodeText(array);
			}
		}
		return result;
	}

	public static bool GetFirstLogOnRewardEarned()
	{
		bool earned = false;
		if (brGetFirstLogOnRewardEarned(out earned) != 0)
		{
			earned = false;
		}
		return earned;
	}

	public static CloudConflictInfo GetUserCacheVariablesCloudConflictInfo()
	{
		if (isBedrockActive())
		{
			CloudConflictInfo cloudConflictInfo = new CloudConflictInfo();
			uint cloudMetadataStringBufferSize = 120u;
			byte[] array = new byte[cloudMetadataStringBufferSize];
			uint deviceNameStringBufferSize = 64u;
			byte[] array2 = new byte[deviceNameStringBufferSize];
			brResult brResult = brGetUserCacheVariablesCloudConflictInfo(ref cloudConflictInfo.connectionStatus, ref cloudConflictInfo.fileConflictStatus, array, ref cloudMetadataStringBufferSize, array2, ref deviceNameStringBufferSize);
			if (brResult == brResult.BR_SUCCESS)
			{
				cloudConflictInfo.metaData = DecodeText(array);
				cloudConflictInfo.deviceName = DecodeText(array2);
				return cloudConflictInfo;
			}
			Debug.Log("Failed to get cloud conflict info for a cloud conflict: " + brResult);
		}
		return null;
	}

	public static bool GetUserCacheVariablesCloudFileInformation(ref brCloudStorageFileInfo fileInfo)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brGetUserCacheVariablesCloudFileInformationUnity(ref fileInfo) == brResult.BR_SUCCESS;
		}
		return result;
	}

	private void BedrockEventNotice(string message)
	{
		Debug.Log("Bedrock event received: " + message);
		switch (message)
		{
		case "BR_FULLY_CONNECTED":
		{
			_hasConnected = true;
			bool earned = false;
			if (brGetFirstLogOnRewardEarned(out earned) != 0)
			{
				earned = false;
			}
			if (Bedrock.UserLogOn != null)
			{
				Bedrock.UserLogOn(this, new LogOnEventArgs(isDeviceAnonymouslyLoggedOn(), earned));
			}
			break;
		}
		case "BR_LOG_ON_FAIL":
			if (Bedrock.UserLogOnFail != null)
			{
				Bedrock.UserLogOnFail(this, new EventArgs());
			}
			break;
		case "BR_DISCONNECTED":
			if (Bedrock.UserLogOff != null)
			{
				Bedrock.UserLogOff(this, new EventArgs());
			}
			break;
		case "BR_PARAMETERS_AVAILABLE":
		{
			float result;
			if (_autoDownloadUserResources && ((DateTime.Now - _lastSwrveUpdateTime).TotalMinutes > (double)_swrveMinimumAutomaticUpdateIntervalInMinutes || _lastSwrveUserId != brGetDefaultOnlineId()) && float.TryParse(CurrentPlatformConfiguration._appVersion, out result))
			{
				float remoteVariableAsFloat = GetRemoteVariableAsFloat("latestAppVersion", result);
				if (result >= remoteVariableAsFloat)
				{
					AsyncRefreshUserResources();
				}
			}
			if (Bedrock.TitleParametersChanged != null)
			{
				Bedrock.TitleParametersChanged(this, new EventArgs());
			}
			break;
		}
		case "BR_USER_ABTEST_PARAMETERS_AVAILABLE":
			_lastSwrveUpdateTime = DateTime.Now;
			_lastSwrveUserId = brGetDefaultOnlineId();
			if (Bedrock.UserResourcesChanged != null)
			{
				Bedrock.UserResourcesChanged(this, new EventArgs());
			}
			break;
		case "BR_BACKGROUND_CONTENT_UNLOCK_SYNC_COMPLETE":
			if (Bedrock.UnlockContentChanged != null)
			{
				Bedrock.UnlockContentChanged(this, new EventArgs());
			}
			break;
		case "BR_USER_VARIABLES_CLOUD_CONFLICT":
			if (Bedrock.UserVarCloudConflict != null)
			{
				Bedrock.UserVarCloudConflict(this, new EventArgs());
			}
			break;
		case "BR_USER_VARIABLES_USER_CHANGED":
			if (Bedrock.UserVarUserChanged != null)
			{
				Bedrock.UserVarUserChanged(this, new EventArgs());
			}
			break;
		case "BR_USER_VARIABLES_UPDATED_FROM_CLOUD":
			if (Bedrock.UserVarUpdatedFromCloud != null)
			{
				Bedrock.UserVarUpdatedFromCloud(this, new EventArgs());
			}
			break;
		case "BR_USER_VARIABLES_CLOUD_PUSH_SUCCEEDED":
			if (Bedrock.UserVarCloudPushSucceeded != null)
			{
				Bedrock.UserVarCloudPushSucceeded(this, new EventArgs());
			}
			break;
		case "BR_USER_VARIABLES_CLOUD_PUSH_FAILED":
			if (Bedrock.UserVarCloudPushFailed != null)
			{
				Bedrock.UserVarCloudPushFailed(this, new EventArgs());
			}
			break;
		case "BR_IAP_CATALOG_REQUEST_COMPLETED":
			if (Bedrock.IAPCatalogRetrieved != null)
			{
				Bedrock.IAPCatalogRetrieved(this, new EventArgs());
			}
			break;
		case "BR_IAP_PURCHASE_REQUEST_COMPLETED":
			if (Bedrock.IAPRequestCompleted != null)
			{
				Bedrock.IAPRequestCompleted(this, new EventArgs());
			}
			break;
		case "BR_FACEBOOK_AUTHORIZE_SUCCESS":
			if (Bedrock.FacebookAuthorizeSuccess != null)
			{
				Bedrock.FacebookAuthorizeSuccess(this, new EventArgs());
			}
			break;
		case "BR_FACEBOOK_AUTHORIZE_FAILURE":
			FacebookEnabled = false;
			if (Bedrock.FacebookAuthorizeFailure != null)
			{
				Bedrock.FacebookAuthorizeFailure(this, new EventArgs());
			}
			break;
		case "BR_ACTIVATE_FACEBOOK_NEEDS_AUTHORIZATION":
			if (Bedrock.FacebookNeedsAuthorization != null)
			{
				Bedrock.FacebookNeedsAuthorization(this, new EventArgs());
			}
			break;
		case "BR_FACEBOOK_REQUEST_SUCCESS":
			if (Bedrock.FacebookRequestSuccess != null)
			{
				Bedrock.FacebookRequestSuccess(this, new EventArgs());
			}
			break;
		case "BR_FACEBOOK_REQUEST_FAILURE":
			if (Bedrock.FacebookRequestFailure != null)
			{
				Bedrock.FacebookRequestFailure(this, new EventArgs());
			}
			break;
		case "BR_USER_CONNECTION_STATUS_CHANGED":
			_connectionStatus = brGetUserConnectionStatus();
			if (Bedrock.UserConnectionStatusChanged != null)
			{
				Bedrock.UserConnectionStatusChanged(null, new EventArgs());
			}
			break;
		case "BR_USERNAME_CHANGED":
			if (Bedrock.UsernameChanged != null)
			{
				Bedrock.UsernameChanged(this, new EventArgs());
			}
			break;
		case "BR_REGISTRATION_REWARD":
			break;
		case "BR_OUTDATED":
			break;
		case "BR_CLOUDSTORAGE_CONNECTED_TO_CLOUD":
			if (Bedrock.CloudStorageConnected != null)
			{
				Bedrock.CloudStorageConnected(this, new EventArgs());
			}
			break;
		case "BR_FRIEND_CACHE_UPDATED":
			if (Bedrock.FriendCacheUpdated != null)
			{
				Bedrock.FriendCacheUpdated(this, new EventArgs());
			}
			break;
		case "BR_SHARED_CREDENTIALS_ACCEPTED":
			if (Bedrock.SharedCredentialsAccepted != null)
			{
				Bedrock.SharedCredentialsAccepted(this, new EventArgs());
			}
			break;
		case "BR_SHARED_CREDENTIALS_DENIED":
			if (Bedrock.SharedCredentialsDenied != null)
			{
				Bedrock.SharedCredentialsDenied(this, new EventArgs());
			}
			break;
		case "BR_MAKE_GOOD_REWARD":
			if (Bedrock.MakeGoodRewardAvailable != null)
			{
				Bedrock.MakeGoodRewardAvailable(this, new EventArgs());
			}
			break;
		case "BR_SWRVE_TALK_MESSAGE_OPENED":
			if (Bedrock.SwrveTalkMessageOpened != null)
			{
				Bedrock.SwrveTalkMessageOpened(this, new EventArgs());
			}
			break;
		case "BR_SWRVE_TALK_MESSAGE_CLOSED":
			if (Bedrock.SwrveTalkMessageClosed != null)
			{
				Bedrock.SwrveTalkMessageClosed(this, new EventArgs());
			}
			break;
		case "BR_EMERGENCY_MESSAGE_AVAILABLE":
			if (Bedrock.EmergencyMessageAvailable != null)
			{
				Bedrock.EmergencyMessageAvailable(this, new EventArgs());
			}
			break;
		case "BR_EMERGENCY_MESSAGE_INVALID":
			if (Bedrock.EmergencyMessageInvalid != null)
			{
				Bedrock.EmergencyMessageInvalid(this, new EventArgs());
			}
			break;
		}
	}

	public void UIClosed(string message)
	{
		Debug.Log("Bedrock UI Close received: " + message);
		brUserInterfaceReasonForClose reason = brUserInterfaceReasonForClose.BR_UI_CLOSE_UNKNOWN;
		switch (message)
		{
		case "BR_UI_STILL_OPEN":
			reason = brUserInterfaceReasonForClose.BR_UI_STILL_OPEN;
			break;
		case "BR_UI_CLOSE_LOGIN":
			reason = brUserInterfaceReasonForClose.BR_UI_CLOSE_LOGIN;
			break;
		case "BR_UI_CLOSE_LOGOUT":
			reason = brUserInterfaceReasonForClose.BR_UI_CLOSE_LOGOUT;
			break;
		case "BR_UI_CLOSE_CANCEL":
			reason = brUserInterfaceReasonForClose.BR_UI_CLOSE_CANCEL;
			break;
		case "BR_UI_CLOSE_TERMINATE_LOGIN":
			reason = brUserInterfaceReasonForClose.BR_UI_CLOSE_TERMINATE_LOGIN;
			break;
		case "BR_UI_CLOSE_UNKNOWN":
			reason = brUserInterfaceReasonForClose.BR_UI_CLOSE_UNKNOWN;
			break;
		case "BR_UI_CLOSE_INVALID_TOKEN":
			reason = brUserInterfaceReasonForClose.BR_UI_CLOSE_INVALID_TOKEN;
			break;
		}
		if (Bedrock.BedrockUIClosed != null)
		{
			Bedrock.BedrockUIClosed(this, new brUserInterfaceReasonForCloseEventArgs(reason));
		}
	}

	public void RegisterForRemoteNotificationsFailed(string message)
	{
		Debug.Log("Received RegisterForRemoteNotificationsFailed: " + message);
	}

	public void HandleRemoteNotificationReceived(string message)
	{
		Debug.Log("RemoteNotificationReceived:" + message);
		if (Bedrock.RemoteNotificationReceived != null)
		{
			Bedrock.RemoteNotificationReceived(this, new RemoteNotificationEventArgs(message));
		}
	}

	public static string GetRemoteVariableAsString(string key, string defaultValue)
	{
		if (isBedrockActive())
		{
			uint num = 1024u;
			uint byteBufferSize = num;
			byte[] array = new byte[byteBufferSize];
			brResult brResult = brGetRemoteVariableAsString(key, array, ref byteBufferSize);
			if (brResult == brResult.BR_BUFFER_TOO_SMALL)
			{
				array = new byte[byteBufferSize];
				brResult = brGetRemoteVariableAsString(key, array, ref byteBufferSize);
			}
			if (brResult == brResult.BR_SUCCESS)
			{
				string text = DecodeText(array);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
		}
		return defaultValue;
	}

	public static int GetRemoteVariableAsInt(string key, int defaultValue)
	{
		if (isBedrockActive())
		{
			int val = defaultValue;
			if (brGetRemoteVariableAsInt(key, out val) == brResult.BR_SUCCESS)
			{
				return val;
			}
		}
		return defaultValue;
	}

	public static float GetRemoteVariableAsFloat(string key, float defaultValue)
	{
		if (isBedrockActive())
		{
			float val = defaultValue;
			if (brGetRemoteVariableAsFloat(key, out val) == brResult.BR_SUCCESS)
			{
				return val;
			}
		}
		return defaultValue;
	}

	public static void Flush()
	{
		if (!isSupportedPlatform() && _UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.Save();
		}
	}

	public static bool UserVariableExists(string key)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			//result = brHasUserCacheVariable(key);
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			result = PlayerPrefs.HasKey(key);
		}
		return result;
	}

	public static bool DeleteUserVariable(string key)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			result = brDeleteUserCacheVariable(key) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.DeleteKey(key);
			return true;
		}
		return result;
	}

	public static bool DeleteAllUserVariables()
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			result = brDeleteAllUserCacheVariables() == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.DeleteAll();
			return true;
		}
		return result;
	}

	public static string GetUserVariableAsString(string key, string defaultValue)
	{
		if (isSupportedPlatform())
		{
			uint num = 1024u;
			uint byteBufferSize = num;
			byte[] array = new byte[byteBufferSize];
			//brResult brResult = brGetUserCacheVariableAsString(key, array, ref byteBufferSize);
			if (byteBufferSize > num)
			{
				array = new byte[byteBufferSize];
				//brResult = brGetUserCacheVariableAsString(key, array, ref byteBufferSize);
			}
			//if (brResult == brResult.BR_SUCCESS)
			//{
				string text = DecodeText(array);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			//}
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms && PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetString(key, defaultValue);
		}
		return defaultValue;
	}

	public static int GetUserVariableAsInt(string key, int defaultValue)
	{
		if (isSupportedPlatform())
		{
			int val = defaultValue;
			/*if (brGetUserCacheVariableAsInt(key, out val) == brResult.BR_SUCCESS)
			{
				return val;
			}*/
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			return PlayerPrefs.GetInt(key, defaultValue);
		}
		return defaultValue;
	}

	public static float GetUserVariableAsFloat(string key, float defaultValue)
	{
		if (isSupportedPlatform())
		{
			float val = defaultValue;
			/*if (brGetUserCacheVariableAsFloat(key, out val) == brResult.BR_SUCCESS)
			{
				return val;
			}*/
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			return PlayerPrefs.GetFloat(key, defaultValue);
		}
		return defaultValue;
	}

	public static bool GetUserVariableAsBool(string key, bool defaultValue)
	{
		if (isSupportedPlatform())
		{
			string userVariableAsString = GetUserVariableAsString(key, "DEFAULT");
			if ("DEFAULT" != userVariableAsString)
			{
				bool result = defaultValue;
				if (bool.TryParse(userVariableAsString, out result))
				{
					return result;
				}
			}
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			string @string = PlayerPrefs.GetString(key, "DEFAULT");
			if ("DEFAULT" != @string)
			{
				bool result2 = defaultValue;
				if (bool.TryParse(@string, out result2))
				{
					return result2;
				}
			}
		}
		return defaultValue;
	}

	public static bool SetUserVariableAsString(string key, string val)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			//result = brSetUserCacheVariableAsString(key, val) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.SetString(key, val);
			return true;
		}
		return result;
	}

	public static bool SetUserVariableAsInt(string key, int val)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			//result = brSetUserCacheVariableAsInt(key, val) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.SetInt(key, val);
			return true;
		}
		return result;
	}

	public static bool SetUserVariableAsFloat(string key, float val)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			//result = brSetUserCacheVariableAsFloat(key, val) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.SetFloat(key, val);
			return true;
		}
		return result;
	}

	public static bool SetUserVariableAsBool(string key, bool val)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			//result = brSetUserCacheVariableAsString(key, val.ToString()) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.SetString(key, val.ToString());
			return true;
		}
		return result;
	}

	public static bool AsyncRefreshUserResources()
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brBeginAsyncRetrieveUserResources() == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool GetRemoteUserResources(string itemId, out Dictionary<string, string> resourceDictionary)
	{
		bool result = false;
		resourceDictionary = null;
		if (isBedrockActive())
		{
			uint byteBufferSize = 4096u;
			byte[] array = new byte[byteBufferSize];
			brResult brResult = brGetRemoteUserResourcesAsJSON(itemId, array, ref byteBufferSize);
			if (brResult == brResult.BR_BUFFER_TOO_SMALL)
			{
				array = new byte[byteBufferSize];
				brResult = brGetRemoteUserResourcesAsJSON(itemId, array, ref byteBufferSize);
			}
			string text = DecodeText(array);
			if (brResult == brResult.BR_SUCCESS && !string.IsNullOrEmpty(text))
			{
				result = true;
				resourceDictionary = new Dictionary<string, string>(0);
				Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
				if (dictionary != null)
				{
					foreach (KeyValuePair<string, object> item in dictionary)
					{
						resourceDictionary[item.Key] = item.Value.ToString();
					}
				}
				else
				{
					Debug.LogError("Failed to parse JSON into dictionary:\n" + text);
				}
			}
		}
		return result;
	}

	public static string GetFromResourceDictionaryAsString(Dictionary<string, string> resourceDictionary, string key, string defaultValue)
	{
		if (resourceDictionary.ContainsKey(key))
		{
			return resourceDictionary[key];
		}
		return defaultValue;
	}

	public static int GetFromResourceDictionaryAsInt(Dictionary<string, string> resourceDictionary, string key, int defaultValue)
	{
		if (resourceDictionary.ContainsKey(key))
		{
			string s = resourceDictionary[key];
			int result = defaultValue;
			if (int.TryParse(s, out result))
			{
				return result;
			}
		}
		return defaultValue;
	}

	public static float GetFromResourceDictionaryAsFloat(Dictionary<string, string> resourceDictionary, string key, float defaultValue)
	{
		if (resourceDictionary.ContainsKey(key))
		{
			string s = resourceDictionary[key];
			float result = defaultValue;
			if (float.TryParse(s, out result))
			{
				return result;
			}
		}
		return defaultValue;
	}

	public static bool GetFromResourceDictionaryAsBool(Dictionary<string, string> resourceDictionary, string key, bool defaultValue)
	{
		if (resourceDictionary.ContainsKey(key))
		{
			string value = resourceDictionary[key];
			bool result = defaultValue;
			if (bool.TryParse(value, out result))
			{
				return result;
			}
		}
		return defaultValue;
	}

	public static bool GetSwrveTalkMessageResult(out brSwrveTalkActionType actionType, out string customActionBuffer, out int customActionBufferSize, out int game)
	{
		actionType = brSwrveTalkActionType.BR_SWRVE_ACTION_NONE;
		game = 0;
		customActionBuffer = string.Empty;
		customActionBufferSize = 0;
		if (isBedrockActive())
		{
			int customActionBufferSize2 = 128;
			byte[] array = new byte[customActionBufferSize2];
			if (brGetSwrveTalkMessageResult(ref actionType, array, ref customActionBufferSize2, ref game) == brResult.BR_SUCCESS)
			{
				customActionBuffer = DecodeText(array);
				return !string.IsNullOrEmpty(customActionBuffer);
			}
		}
		return false;
	}

	public static bool ClearSwrveTalkMessageResult()
	{
		if (isBedrockActive())
		{
			return brClearSwrveTalkMessageResult() == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name)
	{
		return AnalyticsLogEvent(name, false);
	}

	public static bool AnalyticsLogEvent(string name, bool trackTime)
	{
		if (isBedrockActive())
		{
			return brAnalyticsLogEvent(name, null, 0, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, string key, string val, bool trackTime)
	{
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair = default(brKeyValuePair);
			brKeyValuePair.key = key;
			brKeyValuePair.val = val;
			brKeyValuePair[] keyValueArray = new brKeyValuePair[1] { brKeyValuePair };
			int arraySize = 1;
			return brAnalyticsLogEvent(name, keyValueArray, arraySize, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, string key1, string val1, string key2, string val2, bool trackTime)
	{
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair = default(brKeyValuePair);
			brKeyValuePair.key = key1;
			brKeyValuePair.val = val1;
			brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
			brKeyValuePair2.key = key2;
			brKeyValuePair2.val = val2;
			return brAnalyticsLogEvent(name, new brKeyValuePair[2] { brKeyValuePair, brKeyValuePair2 }, 2, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, string key1, string val1, string key2, string val2, string key3, string val3, bool trackTime)
	{
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair = default(brKeyValuePair);
			brKeyValuePair.key = key1;
			brKeyValuePair.val = val1;
			brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
			brKeyValuePair2.key = key2;
			brKeyValuePair2.val = val2;
			brKeyValuePair brKeyValuePair3 = default(brKeyValuePair);
			brKeyValuePair3.key = key3;
			brKeyValuePair3.val = val3;
			return brAnalyticsLogEvent(name, new brKeyValuePair[3] { brKeyValuePair, brKeyValuePair2, brKeyValuePair3 }, 3, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, brKeyValueArray parameters, bool trackTime)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsLogEvent(name, parameters.pairs, parameters.size, trackTime) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsLogVirtualPurchase(string itemID, ulong itemCost, ulong quantity, string virtualCurrencyType)
	{
		brKeyValueArray parameters = default(brKeyValueArray);
		parameters.size = 0;
		parameters.pairs = new brKeyValuePair[0];
		return AnalyticsLogVirtualPurchase(itemID, itemCost, quantity, virtualCurrencyType, parameters);
	}

	public static bool AnalyticsLogVirtualPurchase(string itemID, ulong itemCost, ulong quantity, string virtualCurrencyType, brKeyValueArray parameters)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsLogVirtualPurchase(itemID, itemCost, quantity, virtualCurrencyType, parameters.pairs, parameters.size) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsLogRealPurchase(ulong realCost, string localCurrencyCode, string paymentProvider, ulong virtualCurrencyAmount, string virtualCurrencyType)
	{
		brKeyValueArray parameters = default(brKeyValueArray);
		parameters.size = 0;
		parameters.pairs = new brKeyValuePair[0];
		return AnalyticsLogRealPurchase(realCost, localCurrencyCode, paymentProvider, virtualCurrencyAmount, virtualCurrencyType, parameters);
	}

	public static bool AnalyticsLogRealPurchase(ulong realCost, string localCurrencyCode, string paymentProvider, ulong virtualCurrencyAmount, string virtualCurrencyType, brKeyValueArray parameters)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsLogRealPurchase(realCost, localCurrencyCode, paymentProvider, virtualCurrencyAmount, virtualCurrencyType, parameters.pairs, parameters.size) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsLogRealPurchase(float realCost, string localCurrencyCode, string paymentProvider, ulong virtualCurrencyAmount, string virtualCurrencyType, brKeyValueArray parameters)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsLogRealPurchaseAsFloat(realCost, localCurrencyCode, paymentProvider, virtualCurrencyAmount, virtualCurrencyType, parameters.pairs, parameters.size) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsLogVirtualCurrencyAwarded(ulong virtualCurrencyAmount, string virtualCurrencyType, string key1, string value1, string key2, string value2)
	{
		brKeyValuePair brKeyValuePair = default(brKeyValuePair);
		brKeyValuePair.key = key1;
		brKeyValuePair.val = value1;
		brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
		brKeyValuePair2.key = key2;
		brKeyValuePair2.val = value2;
		int num = 2;
		brKeyValuePair[] array = new brKeyValuePair[num];
		array[0] = brKeyValuePair;
		array[1] = brKeyValuePair2;
		brKeyValueArray parameters = default(brKeyValueArray);
		parameters.pairs = array;
		parameters.size = num;
		return AnalyticsLogVirtualCurrencyAwarded(virtualCurrencyAmount, virtualCurrencyType, parameters);
	}

	public static bool AnalyticsLogVirtualCurrencyAwarded(ulong virtualCurrencyAmount, string virtualCurrencyType, brKeyValueArray parameters)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsLogVirtualCurrencyAwarded(virtualCurrencyAmount, virtualCurrencyType, parameters.pairs, parameters.size) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsSetCustomUserInformation(string key, string val)
	{
		bool result = false;
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair = default(brKeyValuePair);
			brKeyValuePair.key = key;
			brKeyValuePair.val = val;
			brKeyValuePair[] keyValueArray = new brKeyValuePair[1] { brKeyValuePair };
			int arraySize = 1;
			result = brAnalyticsSetCustomUserInformation(keyValueArray, arraySize) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsSetCustomUserInformation(brKeyValueArray parameters)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsSetCustomUserInformation(parameters.pairs, parameters.size) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsEndTimedEvent(string name)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsEndTimedEvent(name) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static short StartUnlockContent(brLobbyServerTier level, string licenseCode)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brUnlockContent(level, licenseCode);
			brStartTask(num);
		}
		return num;
	}

	public static brContentUnlockInfo[] ListUnlockedContent(brLobbyServerTier level, uint maxContentKeys)
	{
		brContentUnlockInfo[] array = new brContentUnlockInfo[0];
		if (isBedrockActive())
		{
			brContentUnlockInfo[] array2 = new brContentUnlockInfo[maxContentKeys];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].init();
			}
			if (brListUnlockedContent(level, array2, maxContentKeys) == brResult.BR_SUCCESS)
			{
				int j;
				for (j = 0; array2[j].contentKey != uint.MaxValue; j++)
				{
				}
				array = new brContentUnlockInfo[j];
				for (int k = 0; k < j; k++)
				{
					array[k] = array2[k];
				}
			}
		}
		return array;
	}

	public static short StartAsyncGetContentKeyFromLicense(brLobbyServerTier level, string licenseCode)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brQueryContentFromLicense(level, licenseCode);
			brStartTask(num);
		}
		return num;
	}

	public static bool FinishAsyncGetContentKeyFromLicense(short taskHandle, ref brContentUnlockInfo contentInfo)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brReadContentKeyFromLicenseQuery(taskHandle, ref contentInfo) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static void StartUnlockedContentCacheUpdateTask(brLobbyServerTier level)
	{
		if (isBedrockActive())
		{
			brUpdateUnlockedContentCache(level);
		}
	}

	public static short StartUnlockedContentSyncWithAnonymousDeviceAccountTask(brLobbyServerTier level)
	{
		short num = -1;
		if (isBedrockActive())
		{
			if (!isDeviceAnonymouslyLoggedOn())
			{
				num = brSynchronizeUnlockedContentWithDeviceAnonymousAccount(level);
				brStartTask(num);
			}
			else
			{
				Debug.Log("StartUnlockedContentSyncWithAnonymousDeviceAccountTask should only be called with from a registered account");
			}
		}
		return num;
	}

	public static bool SetCloudStorageParameters(ref brCloudStorageParameters cloudStorageParameters)
	{
		bool result = false;
		if (isBedrockActive())
		{
			//result = brSetCloudStorageParameters(ref cloudStorageParameters) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool MoveAnonymousUserCacheDataToUser()
	{
		bool flag = false;
		if (isBedrockActive())
		{
			brResult brResult = brMoveAnonymousUserCacheDataToUser();
			flag = brResult == brResult.BR_SUCCESS;
			if (!flag)
			{
				Debug.Log("Failed to move the anonymous user cache data to the current logged on user. " + brResult);
			}
		}
		return flag;
	}

	public static bool ResolveUserCacheVariablesWithCloud(bool resolveWithLocal)
	{
		bool result = false;
		if (isBedrockActive())
		{
			if (resolveWithLocal)
			{
				brResult brResult = brResolveUserCacheVariablesWithCloud(brCloudStorageFileResolveAction.BR_CLOUDSTORAGE_RESOLVE_WITH_LOCAL_FILE);
				result = brResult == brResult.BR_SUCCESS;
				if (brResult != 0)
				{
					Debug.Log("Failed to resolve a cloud storage conflict by selecting the local file and pushing it to the cloud.");
				}
			}
			else
			{
				brResult brResult2 = brResolveUserCacheVariablesWithCloud(brCloudStorageFileResolveAction.BR_CLOUDSTORAGE_RESOLVE_WITH_CLOUD_FILE);
				result = brResult2 == brResult.BR_SUCCESS;
				if (brResult2 != 0)
				{
					Debug.Log("Failed to resolve a cloud storage conflict by selecting the online file and pulling it from the cloud.");
				}
			}
		}
		return result;
	}

	public static brIAPAvailabilityStatus GetIAPAvailabilityStatus()
	{
		return brGetInAppPurchasingAvailability();
	}

	public static bool SetInAppPurchasingReceiptVerificationBehavior(brIAPReceiptBehavior receiptBehavior)
	{
		if (isBedrockActive())
		{
			//brResult brResult = brSetInAppPurchasingReceiptVerificationBehavior(receiptBehavior);
			//return brResult == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool InitializeIAPCatalog(string[] productIdArray, brIAPProductCategory[] productCategoryArray, uint catalogEntryCount)
	{
		if (isBedrockActive())
		{
			brResult brResult = brInitializeInAppPurchasingCatalog(productIdArray, productCategoryArray, catalogEntryCount);
			return brResult == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool SetInAppPurchasingCatalogEntryVirtualCurrencyInfo(string productId, string virtualCurrencyName, ulong virtualCurrencyAmount)
	{
		if (isBedrockActive())
		{
			brResult brResult = brSetInAppPurchaseCatalogEntryVirtualCurrencyInfo(productId, virtualCurrencyName, virtualCurrencyAmount);
			return brResult == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static IAPCatalogEntry GetIAPCatalogEntry(string productId)
	{
		if (isBedrockActive())
		{
			brIAPCatalogEntry catalogEntry = default(brIAPCatalogEntry);
			brResult taskResult = brGetInAppPurchasingCatalogEntryData(productId, ref catalogEntry);
			if (CheckTaskResult(taskResult))
			{
				return new IAPCatalogEntry(catalogEntry);
			}
		}
		return null;
	}

	public static short RequestInAppPurchase(string productId)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brRequestInAppPurchase(productId);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short ValidateLastInAppPurchaseReceipt()
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brValidateLastInAppPurchaseReceipt();
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static bool GetInAppPurchasingStoredCompletedPurchaseCount(out uint numberOfItems)
	{
		if (isBedrockActive())
		{
			brResult taskResult = brGetInAppPurchasingStoredCompletedPurchaseCount(out numberOfItems);
			return CheckTaskResult(taskResult);
		}
		numberOfItems = 0u;
		return false;
	}

	public static bool GetInAppPurchasingFirstCompletedStoredPurchase(out IAPCatalogEntry catalogEntry)
	{
		catalogEntry = null;
		if (isBedrockActive())
		{
			brIAPCatalogEntry catalogEntry2 = default(brIAPCatalogEntry);
			brResult taskResult = brGetInAppPurchasingFirstCompletedStoredPurchase(ref catalogEntry2);
			if (CheckTaskResult(taskResult))
			{
				catalogEntry = new IAPCatalogEntry(catalogEntry2);
				return true;
			}
		}
		return false;
	}

	public static bool ClearInAppPurchasingFirstCompletedStoredPurchase()
	{
		if (isBedrockActive())
		{
			brResult taskResult = brClearInAppPurchasingFirstCompletedStoredPurchase();
			return CheckTaskResult(taskResult);
		}
		return false;
	}

	public static bool getTaskStatus(short taskHandle, out brTaskStatus status)
	{
        status = default(brTaskStatus);
        return isBedrockActive() && brGetTaskStatus(taskHandle, out status) == brResult.BR_SUCCESS;
	}

	public static void ReleaseTaskHandle(ref short taskHandle)
	{
		if (isBedrockActive())
		{
			brEndTask(ref taskHandle);
		}
	}

	public static int getTaskErrorCode(short taskHandle)
	{
		int errorCode = 0;
		if (isBedrockActive())
		{
			brGetTaskErrorCode(taskHandle, ref errorCode);
		}
		return errorCode;
	}

	public static bool Reconnect()
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brReconnect() == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool isUserConnected()
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brGetConnected();
		}
		return result;
	}

	public static string getUsername()
	{
		if (isBedrockActive())
		{
			uint num = 64u;
			byte[] array = new byte[num];
			brResult brResult = brGetUsername(array, num);
			if (brResult == brResult.BR_SUCCESS)
			{
				string text = DecodeText(array);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			Debug.Log("Failed to get the username of the logged in user. Error: " + brResult);
		}
		return null;
	}

	public static brUserConnectionStatus getUserConnectionStatus()
	{
		brUserConnectionStatus result = brUserConnectionStatus.BR_LOGGED_OUT;
		if (isBedrockActive())
		{
			result = _connectionStatus;
		}
		return result;
	}

	public static bool StartDeviceAnonymousLogOn()
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brDeviceAnonymousLogOn() == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool isDeviceAnonymouslyLoggedOn()
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brGetLoggedOnAnonymously();
		}
		return result;
	}

	public static ulong getDefaultOnlineId()
	{
		ulong result = 0uL;
		if (isBedrockActive())
		{
			result = brGetDefaultOnlineId();
		}
		return result;
	}

	public static bool CheckSharedCredentials()
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brCheckSharedCredentials() == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool GetApplicationInstalled(brBedrockGameId application)
	{
		if (isBedrockActive())
		{
			return brGetApplicationInstalled(application);
		}
		return false;
	}

	public static bool displayUserInterface(brUserInterfaceScreen userInterface)
	{
		if (isBedrockActive())
		{
			return CheckTaskResult(brDisplayUserInterfaceUnity((int)userInterface));
		}
		return false;
	}

	public static short SendChallengeNotification(ulong[] targetIds, uint challengeId, string challengeDescription, int badge, string sound, byte[] payload)
	{
		short num = -1;
		if (isBedrockActive())
		{
			int numTargets = targetIds.Length;
			num = brSendChallengeNotification(targetIds, numTargets, challengeId, challengeDescription, badge, sound, payload);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static bool GetNumChallenges(out uint numChallenges)
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			taskResult = brGetNumChallenges(out numChallenges);
		}
		else
		{
			numChallenges = 0u;
		}
		return CheckTaskResult(taskResult);
	}

	public static bool GetChallengeInfo(uint challengeId, out brChallengeInfo challengeInfo)
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			taskResult = brCopyChallengeInfo(challengeId, out challengeInfo);
		}
		else
		{
			challengeInfo = default(brChallengeInfo);
		}
		return CheckTaskResult(taskResult);
	}

	public static short StartChallengeStatusRequest(uint challengeId)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brGetChallengeStatus(challengeId, IntPtr.Zero);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static bool GetChallengeStatusResult(short taskHandle, out brChallengeStatus status)
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			taskResult = brGetChallengeStatusResult(taskHandle, out status);
		}
		else
		{
			status = brChallengeStatus.BR_CHALLENGE_UNKNOWN;
		}
		return CheckTaskResult(taskResult);
	}

	public static short UpdateStatusOfAllChallenges()
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brUpdateStatusOfAllChallenges(IntPtr.Zero);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short StartServerTimeRequest(brLobbyServerTier level)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brGetServerTime(level, IntPtr.Zero);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static bool GetServerTimeResult(short taskHandle, out uint time)
	{
		time = 0u;
		brResult brResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			brResult = brGetServerTimeTaskResult(taskHandle, out time);
			if (brResult == brResult.BR_SUCCESS && (bool)_instance)
			{
				DateTime dateTime = new DateTime(1970, 1, 1).AddSeconds(time);
				_instance._serverTimeOffset = DateTime.UtcNow - dateTime;
				_instance._lastServerTimeUpdate = DateTime.Now;
			}
		}
		return CheckTaskResult(brResult);
	}

	public static short StartWriteToLeaderboardRequest(brLobbyServerTier tier, brLeaderboardRow[] rows)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brWriteLeaderboardRow(tier, rows, rows.Length);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short StartReadLeaderboardByPivot(brLobbyServerTier tier, uint leaderboardId, uint maxResults, bool friendsOnly, bool retrieveDisplayName)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brReadLeaderboardByPivot(tier, leaderboardId, IntPtr.Zero, maxResults, friendsOnly);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short StartReadLeaderboardByGivenPivot(brLobbyServerTier tier, uint leaderboardId, ulong userId, uint maxResults, bool friendsOnly)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brReadLeaderboardByGivenPivot(tier, leaderboardId, userId, IntPtr.Zero, maxResults, friendsOnly);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short StartReadLeaderboardByRank(brLobbyServerTier tier, uint leaderboardId, ulong rank, uint maxResults, bool friendsOnly, bool retrieveDisplayName)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brReadLeaderboardByRank(tier, leaderboardId, rank, IntPtr.Zero, maxResults, friendsOnly);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short StartReadLeaderboardByRating(brLobbyServerTier tier, uint leaderboardId, ulong rating, uint maxResults, bool friendsOnly)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brReadLeaderboardByRating(tier, leaderboardId, rating, IntPtr.Zero, maxResults, friendsOnly);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short StartReadLeaderboardByUserIds(brLobbyServerTier tier, uint leaderboardId, ulong[] userIds, uint maxResults, bool friendsOnly, bool retrieveDisplayName)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brReadLeaderboardByUserIds(tier, leaderboardId, userIds, IntPtr.Zero, maxResults, friendsOnly);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static bool GetLeaderboardResults(short taskHandle, brLeaderboardRow[] rows, uint maxResults, bool friendsOnly)
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			taskResult = brLeaderboardGetResultsFromTask(taskHandle, rows, maxResults, friendsOnly);
		}
		return CheckTaskResult(taskResult);
	}

	public static bool FacebookPostToWall(brFacebookPostParameters post)
	{
		if (isBedrockActive())
		{
			brResult taskResult = brFacebookPostToWall(ref post);
			return CheckTaskResult(taskResult);
		}
		return false;
	}

	public static bool FacebookPostAction(brFacebookActionParameters action)
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			taskResult = brFacebookPostAction(ref action);
		}
		return CheckTaskResult(taskResult);
	}

	public static bool FacebookRequestReadPermissions()
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			brFacebookReadPermissions permissions = default(brFacebookReadPermissions);
			taskResult = brFacebookRequestReadPermissions(ref permissions);
		}
		return CheckTaskResult(taskResult);
	}

	public static bool FacebookRequestPublishPermissions(bool publishActions)
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			brFacebookPublishPermissions permissions = default(brFacebookPublishPermissions);
			permissions.publishActions = (publishActions ? 1 : 0);
			taskResult = brFacebookRequestPublishPermissions(ref permissions);
		}
		return CheckTaskResult(taskResult);
	}

	public static bool UpdateFriendsList()
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			taskResult = brUpdateFriendsList();
		}
		return CheckTaskResult(taskResult);
	}

	public static short StartGetIncomingFriendInvites(uint maxInvites)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brGetIncomingFriendInvites(IntPtr.Zero, IntPtr.Zero, maxInvites);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static bool GetIncomingFriendInvites(short taskHandle, brFriendInvite[] incomingInvites, out uint numInvitesReceived, uint maxInvites)
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			taskResult = brGetIncomingFriendInvitesFromTask(taskHandle, incomingInvites, out numInvitesReceived, maxInvites);
		}
		else
		{
			numInvitesReceived = 0u;
		}
		return CheckTaskResult(taskResult);
	}

	public static bool GetNumIncomingFriendInvites(short taskHandle, bool ignoreAutoInvites, out uint numInvitesReceived)
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			taskResult = brGetNumIncomingFriendInvitesFromTask(taskHandle, ignoreAutoInvites, out numInvitesReceived);
		}
		else
		{
			numInvitesReceived = 0u;
		}
		return CheckTaskResult(taskResult);
	}

	public static bool GetFriendsWithCurrentGame(brFriendInfo[] friendsList, ref uint numFriends)
	{
		brResult taskResult = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			taskResult = brGetFriendsWithCurrentGame(friendsList, (uint)friendsList.Length, ref numFriends);
		}
		return CheckTaskResult(taskResult);
	}

	public static short GetLinkedAccounts(uint accountTypes)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brGetLinkedAccounts(accountTypes);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static bool GetLinkedAccountsResult(short taskHandle, out brLinkedAccountsInfo accountInfo)
	{
		bool result = false;
		accountInfo = default(brLinkedAccountsInfo);
		if (isBedrockActive())
		{
			brResult taskResult = brGetLinkedAccountsInfo(taskHandle, ref accountInfo);
			result = CheckTaskResult(taskResult);
		}
		return result;
	}

	public static bool GetMakeGoodReward(out MakeGoodRewardInfo reward)
	{
		reward = null;
		if (isBedrockActive())
		{
			brMakeGoodRewardInfo reward2 = default(brMakeGoodRewardInfo);
			brResult taskResult = brGetMakeGoodReward(ref reward2);
			if (CheckTaskResult(taskResult))
			{
				reward = new MakeGoodRewardInfo(reward2);
				return true;
			}
		}
		return false;
	}

	public static bool ClearMakeGoodReward()
	{
		return isBedrockActive() && CheckTaskResult(brClearMakeGoodReward());
	}

	public static short GetMakeGoodSupportCode()
	{
		if (!isBedrockActive())
		{
			return -1;
		}
		short num = brGetSupportCode();
		if (num != -1)
		{
			brStartTask(num);
		}
		return num;
	}

	public static bool GetMakeGoodSupportCodeTaskResult(short taskHandle, out string supportCode)
	{
		supportCode = null;
		if (isBedrockActive())
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			brResult taskResult = brGetSupportCodeTaskResult(taskHandle, stringBuilder, (uint)stringBuilder.Capacity);
			if (CheckTaskResult(taskResult))
			{
				supportCode = stringBuilder.ToString();
				return true;
			}
		}
		return false;
	}

	public static short UpdateSharedContentUsageForUser(brBedrockGameId gameId, int contentType, int contentSubType, uint serverTimeStamp)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brUpdateSharedContentUsageForUser(gameId, contentType, contentSubType, serverTimeStamp, IntPtr.Zero, 0u);
			if (num != -1)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static bool HasEmegencyMessage()
	{
		if (isBedrockActive())
		{
			bool hasMessage;
			brResult taskResult = brHasEmergencyMessage(out hasMessage);
			if (CheckTaskResult(taskResult))
			{
				return hasMessage;
			}
		}
		return false;
	}

	public static bool TryGetEmergencyMessage(string languageCode, out string message)
	{
		message = null;
		if (isBedrockActive())
		{
			uint messageBufferSize = 255u;
			StringBuilder stringBuilder = new StringBuilder((int)messageBufferSize);
			brResult brResult = brGetEmergencyMessage(languageCode, stringBuilder, ref messageBufferSize);
			if (brResult == brResult.BR_BUFFER_TOO_SMALL)
			{
				stringBuilder = new StringBuilder((int)messageBufferSize);
				brResult = brGetEmergencyMessage(languageCode, stringBuilder, ref messageBufferSize);
			}
			if (CheckTaskResult(brResult) || brResult == brResult.BR_BUFFER_TOO_SMALL)
			{
				if (brResult == brResult.BR_BUFFER_TOO_SMALL)
				{
					Debug.LogWarning(string.Format("Emergency message buffer was too small (required {0} characters), message truncated required.", messageBufferSize));
				}
				message = stringBuilder.ToString();
				return true;
			}
		}
		return false;
	}

	public static bool CheckForNewEmergencyMessage()
	{
		return isBedrockActive() && CheckTaskResult(brCheckForNewEmergencyMessage());
	}

	private static bool CheckTaskResult(brResult taskResult)
	{
		bool flag = taskResult == brResult.BR_SUCCESS;
		if (!flag)
		{
			Debug.LogError("Task failed with result: " + taskResult);
		}
		return flag;
	}

	public static string WebsiteGetSubDir()
	{
		uint byteBufferSize = 32u;
		byte[] array = new byte[byteBufferSize];
		if (brWebsiteGetSubDir(array, ref byteBufferSize) == brResult.BR_SUCCESS)
		{
			return DecodeText(array);
		}
		return null;
	}

	public static bool WebsiteSetSubDir(string subDir)
	{
		return brWebsiteSetSubDir(subDir) == brResult.BR_SUCCESS;
	}
}
