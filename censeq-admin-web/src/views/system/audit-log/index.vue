<template>
  <div class="audit-log-container layout-padding">
    <div class="audit-log-padding layout-padding-auto layout-padding-view">
      <div class="search-bar mb15">
        <el-date-picker
          v-model="state.searchDateRange"
          type="datetimerange"
          range-separator="至"
          start-placeholder="开始时间"
          end-placeholder="结束时间"
          value-format="YYYY-MM-DD HH:mm:ss"
          size="default"
        />
        <el-input v-model="state.searchParams.userName" placeholder="用户名" size="default" clearable style="width: 150px" />
        <el-input v-model="state.searchParams.url" placeholder="URL" size="default" clearable style="width: 200px" />
        <el-select v-model="state.searchParams.httpMethod" placeholder="HTTP方法" size="default" clearable style="width: 120px">
          <el-option label="GET" value="GET" />
          <el-option label="POST" value="POST" />
          <el-option label="PUT" value="PUT" />
          <el-option label="DELETE" value="DELETE" />
        </el-select>
        <el-select v-model="state.searchParams.hasException" placeholder="是否有异常" size="default" clearable style="width: 130px">
          <el-option label="正常" :value="false" />
          <el-option label="异常" :value="true" />
        </el-select>
        <el-button type="primary" size="default" @click="onQuery">
          <el-icon><ele-Search /></el-icon>
          查询
        </el-button>
        <el-button size="default" @click="onReset">重置</el-button>
      </div>

      <el-table :data="state.tableData.data" v-loading="state.tableData.loading" style="width: 100%">
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="executionTime" label="执行时间" width="160" show-overflow-tooltip>
          <template #default="scope">
            {{ formatDateTime(scope.row.executionTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="userName" label="用户名" width="100" show-overflow-tooltip />
        <el-table-column prop="httpMethod" label="HTTP方法" width="90" align="center">
          <template #default="scope">
            <el-tag size="small" :type="getHttpMethodType(scope.row.httpMethod)">
              {{ scope.row.httpMethod }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="url" label="URL" min-width="200" show-overflow-tooltip />
        <el-table-column prop="clientIpAddress" label="IP地址" width="120" show-overflow-tooltip />
        <el-table-column prop="executionDuration" label="执行时长" width="90" align="center">
          <template #default="scope">
            <el-tag size="small" :type="scope.row.executionDuration > 1000 ? 'warning' : 'success'">
              {{ scope.row.executionDuration }}ms
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="hasException" label="状态" width="80" align="center">
          <template #default="scope">
            <el-tag size="small" :type="scope.row.hasException ? 'danger' : 'success'">
              {{ scope.row.hasException ? '异常' : '正常' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" align="center" fixed="right">
          <template #default="scope">
            <el-button size="small" text type="primary" @click="onViewDetail(scope.row)">详情</el-button>
            <el-popconfirm title="确定删除该审计日志吗？" @confirm="onDelete(scope.row)">
              <template #reference>
                <el-button size="small" text type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <el-pagination
        @size-change="onHandleSizeChange"
        @current-change="onHandleCurrentChange"
        class="mt15"
        :pager-count="5"
        :page-sizes="[10, 20, 50, 100]"
        v-model:current-page="state.tableData.param.pageIndex"
        background
        v-model:page-size="state.tableData.param.pageSize"
        layout="total, sizes, prev, pager, next, jumper"
        :total="state.tableData.total"
      />
    </div>

    <!-- 详情对话框 -->
    <AuditLogDetailDialog ref="detailDialogRef" />
  </div>
</template>

<script setup lang="ts" name="systemAuditLog">
import { defineAsyncComponent, reactive, onMounted, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { useAuditLogApi } from '/@/api/apis';
import { AuditLogDto } from '/@/api/models/audit-logging';

const AuditLogDetailDialog = defineAsyncComponent(() => import('./detail.vue'));

const detailDialogRef = ref();

const state = reactive({
  searchDateRange: [] as string[],
  searchParams: {
    userName: '',
    url: '',
    httpMethod: '',
    hasException: undefined as boolean | undefined,
  },
  tableData: {
    data: [] as Array<AuditLogDto>,
    total: 0,
    loading: false,
    param: {
      pageIndex: 1,
      pageSize: 20,
    },
  },
});

// 格式化日期时间
const formatDateTime = (dateStr: string) => {
  if (!dateStr) return '';
  const date = new Date(dateStr);
  return date.toLocaleString('zh-CN');
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

// 初始化表格数据
const getTableData = async () => {
  state.tableData.loading = true;
  try {
    const { getAuditLogPage } = useAuditLogApi();
    const data = await getAuditLogPage({
      skipCount: (state.tableData.param.pageIndex - 1) * state.tableData.param.pageSize,
      maxResultCount: state.tableData.param.pageSize,
      startTime: state.searchDateRange?.[0],
      endTime: state.searchDateRange?.[1],
      userName: state.searchParams.userName || undefined,
      url: state.searchParams.url || undefined,
      httpMethod: state.searchParams.httpMethod || undefined,
      hasException: state.searchParams.hasException,
    });
    state.tableData.data = data.items ?? [];
    state.tableData.total = data.totalCount ?? 0;
  } finally {
    state.tableData.loading = false;
  }
};

// 查询
const onQuery = () => {
  state.tableData.param.pageIndex = 1;
  getTableData();
};

// 重置
const onReset = () => {
  state.searchDateRange = [];
  state.searchParams = {
    userName: '',
    url: '',
    httpMethod: '',
    hasException: undefined,
  };
  onQuery();
};

// 查看详情
const onViewDetail = (row: AuditLogDto) => {
  detailDialogRef.value.openDialog(row);
};

// 删除
const onDelete = async (row: AuditLogDto) => {
  try {
    const { deleteAuditLog } = useAuditLogApi();
    await deleteAuditLog(row.id);
    ElMessage.success('删除成功');
    await getTableData();
  } catch (error) {
    // 删除失败
  }
};

// 分页改变
const onHandleSizeChange = (val: number) => {
  state.tableData.param.pageSize = val;
  getTableData();
};

// 分页改变
const onHandleCurrentChange = (val: number) => {
  state.tableData.param.pageIndex = val;
  getTableData();
};

// 页面加载时
onMounted(() => {
  getTableData();
});
</script>

<style scoped lang="scss">
.audit-log-container {
  .audit-log-padding {
    padding: 15px;
    .el-table {
      flex: 1;
    }
  }
  .search-bar {
    display: flex;
    gap: 10px;
    flex-wrap: wrap;
    align-items: center;
  }
}
</style>
