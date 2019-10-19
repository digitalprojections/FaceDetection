//////////////////////////////////////////////////////////////////////////////////////////
//	< DispApiIO.h > DispApi アクセス関連ヘッダーファイル
//
//	DispAPI アクセス関連を定義します
//
//	最終更新日付 : 2013.12.20
//	最終更新者	 : K.Tani
//
// Revision Histroy:
// [No]	[Date]		[Name]				[Description]
// 0000	2013.10.28	H.Mori				Create
// 0001	2013.12.20	K.Tani				Remove atlstr.h
//										Move #define to dispapiio_def.h
//
//				Copyright (C) 2013 Digital Electronics Corporation. All Right Reserved.
//////////////////////////////////////////////////////////////////////////////////////////
#pragma once

#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>
#include <guiddef.h>
#include <cstringt.h>

#define DISPAPIIO_API __declspec(dllexport)

/////////////////////////////////////////////////////////////////////////////////////////
//	プロトタイプ宣言
/////////////////////////////////////////////////////////////////////////////////////////
DISPAPIIO_API BOOL WINAPI DispDeviceOpen( void );
DISPAPIIO_API BOOL WINAPI DispDeviceClose( void );
DISPAPIIO_API BOOL WINAPI DispResetPipe( void );
DISPAPIIO_API BOOL WINAPI DispResetDevice( void );
