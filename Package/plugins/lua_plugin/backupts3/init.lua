--
-- Testmodule initialisation, this script is called via autoload mechanism when the
-- TeamSpeak 3 client starts.
--

require("ts3init")            -- Required for ts3RegisterModule
require("backupts3/events")  -- Forwarded TeamSpeak 3 callbacks

local MODULE_NAME = "backupts3"

--
-- Initialize menus. Optional function, if not using menus do not implement this.
-- This function is called automatically by the TeamSpeak client.
--
local function createMenus(moduleMenuItemID)
    -- Store value added to menuIDs to be able to calculate menuIDs for this module again for setPluginMenuEnabled (see demo.lua)
    backupts3_events.moduleMenuItemID = moduleMenuItemID

    -- Create the menus
    -- There are three types of menu items:
    --   ts3defs.PluginMenuType.PLUGIN_MENU_TYPE_CLIENT:  Client context menu
    --   ts3defs.PluginMenuType.PLUGIN_MENU_TYPE_CHANNEL: Channel context menu
    --   ts3defs.PluginMenuType.PLUGIN_MENU_TYPE_GLOBAL:  "Plugins" menu in menu bar of main window
    --
    -- Menu IDs are used to identify the menu item when onMenuItemEvent is called, see testmodule/events.lua for the implementation of onMenuItemEvent
    -- Valid menu IDs are 0 to 999.
    --
    -- The menu text is required, max length is 128 characters
    --
    -- The icon is optional, max length is 128 characters. When not using icons, just pass an empty string.
    -- Icons are loaded from a subdirectory in the TeamSpeak client plugins folder. The subdirectory must be named like the
    -- plugin filename, without dll/so/dylib suffix
    -- e.g. for "test_plugin.dll", icon "1.png" is loaded from <TeamSpeak 3 Client install dir>\plugins\test_plugin\1.png
    -- In this example we reuse the existing icons from the plugins\test_plugin\ directory, which resides as "..\test_plugin" relative to
    -- the lua_plugin directory.

    return {
        {ts3defs.PluginMenuType.PLUGIN_MENU_TYPE_GLOBAL,  backupts3_events.MenuIDs.MENU_ID_GLOBAL_1,  "Backup Plugin",  "backupts3/res/BackupRestore.png"},
    }
end

-- Define which callbacks you want to receive in your module. Callbacks not mentioned
-- here will not be called. To avoid function name collisions, your callbacks should
-- be put into an own package.
local registeredEvents = {
    createMenus = createMenus,
    onConnectStatusChangeEvent = backupts3_events.onConnectStatusChangeEvent,
    onNewChannelEvent = backupts3_events.onNewChannelEvent,
    onTalkStatusChangeEvent = backupts3_events.onTalkStatusChangeEvent,
    onTextMessageEvent = backupts3_events.onTextMessageEvent,
    onPluginCommandEvent = backupts3_events.onPluginCommandEvent,
    onMenuItemEvent = backupts3_events.onMenuItemEvent
}

-- Register your callback functions with a unique module name.
ts3RegisterModule(MODULE_NAME, registeredEvents)
