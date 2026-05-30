export interface SystemBaseInfoDto {
	hostName: string;
	systemOs: string;
	osArchitecture: string;
	processorCount: string;
	sysRunTime: string;
	remoteIp?: string | null;
	localIp?: string | null;
	frameworkDescription: string;
	environment: string;
	wwwroot?: string | null;
	stage: string;
}

export interface SystemUsageInfoDto {
	freeRam: string;
	usedRam: string;
	totalRam: string;
	ramRate: string;
	cpuRate: string;
	startTime: string;
	runTime: string;
}

export interface SystemDiskInfoDto {
	diskName: string;
	diskType: string;
	totalSize: number;
	used: number;
	availableFreeSpace: number;
	usedPercent: number;
}

export interface AssemblyInfoDto {
	name: string;
	version: string;
}
