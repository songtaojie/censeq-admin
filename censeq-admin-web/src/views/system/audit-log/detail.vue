<template>
  <el-dialog title="审计日志详情" v-model="state.dialogVisible" width="900px" destroy-on-close>
    <el-descriptions :column="2" border v-if="state.auditLog">
      <el-descriptions-item label="用户名称">{{ state.auditLog.userName || '-' }}</el-descriptions-item>
      <el-descriptions-item label="租户名称">{{ state.auditLog.tenantName || '-' }}</el-descriptions-item>
      <el-descriptions-item label="HTTP方法">
        <el-tag size="small" :type="getHttpMethodType(state.auditLog.httpMethod)">{{ state.auditLog.httpMethod || '-' }}</el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="HTTP状态码">
        <el-tag size="small" :type="getStatusCodeType(state.auditLog.httpStatusCode)">{{ state.auditLog.httpStatusCode || '-' }}</el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="URL" :span="2">{{ state.auditLog.url || '-' }}</el-descriptions-item>
      <el-descriptions-item label="客户端IP">{{ state.auditLog.clientIpAddress || '-' }}</el-descriptions-item>
      <el-descriptions-item label="客户端名称">{{ state.auditLog.clientName || '-' }}</el-descriptions-item>
      <el-descriptions-item label="执行时间">{{ formatDateTime(state.auditLog.executionTime) }}</el-descriptions-item>
      <el-descriptions-item label="执行时长">
        <el-tag size="small" :type="state.auditLog.executionDuration > 1000 ? 'warning' : 'success'">{{ state.auditLog.executionDuration }}ms</el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="浏览器信息" :span="2">{{ state.auditLog.browserInfo || '-' }}</el-descriptions-item>
      <el-descriptions-item label="关联ID" :span="2">{{ state.auditLog.correlationId || '-' }}</el-descriptions-item>
    </el-descriptions>

    <el-tabs v-model="state.activeTab" class="detail-tabs" v-if="state.auditLog">
      <el-tab-pane label="操作方法" name="actions" v-if="state.auditLog.actions?.length">
        <el-timeline>
          <el-timeline-item v-for="action in state.auditLog.actions" :key="action.id" :timestamp="formatDateTime(action.executionTime)">
            <el-card>
              <p><strong>服务：</strong>{{ action.serviceName }}</p>
              <p><strong>方法：</strong>{{ action.methodName }}</p>
              <p><strong>执行时长：</strong>{{ action.executionDuration }}ms</p>
              <el-collapse v-if="action.parameters">
                <el-collapse-item title="参数">
                  <pre>{{ formatJson(action.parameters) }}</pre>
                </el-collapse-item>
              </el-collapse>
            </el-card>
          </el-timeline-item>
        </el-timeline>
      </el-tab-pane>
      <el-tab-pane label="实体变更" name="entityChanges" v-if="state.auditLog.entityChanges?.length">
        <el-timeline>
          <el-timeline-item v-for="change in state.auditLog.entityChanges" :key="change.id" :type="getChangeTypeColor(change.changeType)">
            <template #dot>
              <el-icon><component :is="getChangeTypeIcon(change.changeType)" /></el-icon>
            </template>
            <el-card>
              <template #header>
                <span>{{ change.entityTypeFullName }} ({{ getChangeTypeText(change.changeType) }})</span>
                <span class="text-gray">ID: {{ change.entityId }}</span>
              </template>
              <el-table :data="change.propertyChanges" size="small" border v-if="change.propertyChanges?.length">
                <el-table-column prop="propertyName" label="属性名" width="150" />
                <el-table-column prop="originalValue" label="原值">
                  <template #default="scope">
                    <span class="text-danger">{{ scope.row.originalValue || '(空)' }}</span>
                  </template>
                </el-table-column>
                <el-table-column prop="newValue" label="新值">
                  <template #default="scope">
                    <span class="text-success">{{ scope.row.newValue || '(空)' }}</span>
                  </template>
                </el-table-column>
              </el-table>
            </el-card>
          </el-timeline-item>
        </el-timeline>
      </el-tab-pane>
      <el-tab-pane label="异常信息" name="exception" v-if="state.auditLog.exceptions">
        <el-alert type="error" :closable="false">
          <pre style="white-space: pre-wrap; word-break: break-all;">{{ state.auditLog.exceptions }}</pre>
        </el-alert>
      </el-tab-pane>
    </el-tabs>

    <template #footer>
      <span class="dialog-footer">
        <el-button @click="state.dialogVisible = false">关闭</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts" name="auditLogDetailDialog">
import { reactive } from 'vue';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { AuditLogDto } from '/@/api/models/audit-logging';

const state = reactive({
  dialogVisible: false,
  activeTab: 'actions',
  auditLog: null as AuditLogDto | null,
});

// 打开对话框
const openDialog = (row: AuditLogDto) => {
  state.auditLog = row;
  state.activeTab = row.actions?.length ? 'actions' : row.entityChanges?.length ? 'entityChanges' : 'exception';
  state.dialogVisible = true;
};

// 格式化日期时间
const formatDateTime = (dateStr?: string) => {
  if (!dateStr) return '-';
  return new Date(dateStr).toLocaleString('zh-CN');
};

// 格式化JSON
const formatJson = (jsonStr?: string) => {
  if (!jsonStr) return '';
  try {
    return JSON.stringify(JSON.parse(jsonStr), null, 2);
  } catch {
    return jsonStr;
  }
};

// 获取HTTP方法标签类型
const getHttpMethodType = (method?: string) => {
  switch (method) {
    case 'GET': return 'success';
    case 'POST': return 'primary';
    case 'PUT': return 'warning';
    case 'DELETE': return 'danger';
    default: return 'info';
  }
};

// 获取状态码标签类型
const getStatusCodeType = (code?: number) => {
  if (!code) return 'info';
  if (code >= 200 && code < 300) return 'success';
  if (code >= 300 && code < 400) return 'warning';
  return 'danger';
};

// 获取变更类型文本
const getChangeTypeText = (type: number) => {
  switch (type) {
    case 0: return '创建';
    case 1: return '更新';
    case 2: return '删除';
    default: return '未知';
  }
};

// 获取变更类型颜色
const getChangeTypeColor = (type: number) => {
  switch (type) {
    case 0: return 'primary';
    case 1: return 'warning';
    case 2: return 'danger';
    default: return 'info';
  }
};

// 获取变更类型图标
const getChangeTypeIcon = (type: number) => {
  switch (type) {
    case 0: return Plus;
    case 1: return Edit;
    case 2: return Delete;
    default: return Edit;
  }
};

defineExpose({ openDialog });
</script>

<style scoped lang="scss">
.detail-tabs {
  margin-top: 20px;
}

.text-gray {
  color: #909399;
  margin-left: 10px;
  font-size: 12px;
}

.text-danger {
  color: #f56c6c;
}

.text-success {
  color: #67c23a;
}

pre {
  background: #f5f7fa;
  padding: 10px;
  border-radius: 4px;
  margin: 0;
}
</style>
