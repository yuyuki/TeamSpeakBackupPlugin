--
-- Testmodule callback functions
--
-- To avoid function name collisions, you should use local functions and export them with a unique package name.
--

local MenuIDs = {
    MENU_ID_GLOBAL_1 = 1,
}

-- Will store factor to add to menuID to calculate the real menuID used in the TeamSpeak client (to support menus from multiple Lua modules)
-- Add this value to above menuID when passing the ID to setPluginMenuEnabled. See demo.lua for an example.
local moduleMenuItemID = 0
local trace = 0

--
-- Called when a plugin menu item (see ts3plugin_initMenus) is triggered. Optional function, when not using plugin menus, do not implement this.
--
-- Parameters:
--  serverConnectionHandlerID: ID of the current server tab
--  type: Type of the menu (ts3defs.PluginMenuType.PLUGIN_MENU_TYPE_CHANNEL, ts3defs.PluginMenuType.PLUGIN_MENU_TYPE_CLIENT or ts3defs.PluginMenuType.PLUGIN_MENU_TYPE_GLOBAL)
--  menuItemID: Id used when creating the menu item
--  selectedItemID: Channel or Client ID in the case of PLUGIN_MENU_TYPE_CHANNEL and PLUGIN_MENU_TYPE_CLIENT. 0 for PLUGIN_MENU_TYPE_GLOBAL.
--
local function onMenuItemEvent(serverConnectionHandlerID, menuType, menuItemID, selectedItemID)
    local configPath = string.gsub(ts3.getConfigPath(), '\\', '\\\\')
    local appPath = string.gsub(ts3.getAppPath(), '\\', '\\\\')
    local exePath = '"' .. ts3.getPluginPath() .. 'lua_plugin/backupts3/BackupSetting.exe" "' .. appPath .. '" "' .. configPath .. '"'
    executeWinForm(exePath)
end

function WriteLog(fct, msg)
    if (trace == 1) then
        ts3.printMessageToCurrentTab('[' .. fct .. '] ' .. msg)
    end
end

function executeWinForm(cmd)
    WriteLog('executeWinForm', cmd)
    local exeCmd = 'start "" ' .. cmd
    WriteLog('executeWinForm', exeCmd)
    os.execute(exeCmd)
end

backupts3_events = {
    MenuIDs = MenuIDs,
    moduleMenuItemID = moduleMenuItemID,
    onMenuItemEvent = onMenuItemEvent
}