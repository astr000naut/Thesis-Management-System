<template>
    <div class="page__container flex-col rg-2">
      <router-view></router-view>
      <div class="page__header flex-row">
        <h1 class="page__title" style="font-size: 24px;">Danh sách Khoa</h1>
        <el-button type="primary" @click="btnAddOnClick">Thêm mới</el-button>
        
      </div>
      <div class="search-container flex-row al-center cg-2">
        <div class="reload-btn">
          <el-button :icon="Refresh" circle @click="btnRefreshOnClick"/>
        </div>
        <div class="search-input">
          <el-input
            v-model="searchText"
            style="width: 240px"
            placeholder="Tìm kiếm"
            :prefix-icon="Search"
            @input="searchTextOnInput"
            clearable
          />
        </div>
      </div>
      <div class="table__container fl-1">
        <el-table :data="tenants" style="width: 100%" row-key="facultyId" v-loading="loading">
          <el-table-column fixed prop="facultyCode" label="Mã khoa" width="150" />
          <el-table-column prop="facultyName" label="Tên khoa"/>
          <el-table-column prop="description" label="Mô tả" width="300" />
          <el-table-column fixed="right" label="Thao tác" width="120">
            <template #default="scope">
              <el-dropdown size="small" split-button type="default"
                @click="btnViewItemOnClick(scope.row)"
              >
                Xem
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item
                      @click="btnEditItemOnClick(scope.row)"
                    >Sửa</el-dropdown-item>
                    <el-dropdown-item
                      @click="btnDeleteItemOnClick(scope.row)"  
                    >Xóa</el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div class="pagination">
        <el-pagination
          :current-page="pageNumber"
          :page-size="pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :small="false"
          :disabled="loading"
          :background="false"
          layout="total, sizes, prev, pager, next"
          :total="total"
          @size-change="tenantStore.setPageSize"
          @current-change="tenantStore.setPageNumber"
        />
      </div>
    </div>
  </template>
  
  
  <script setup>
      import {onMounted, ref} from 'vue';
      import { useTenantStore } from '@/stores';
      import { useRouter } from 'vue-router';
      import { storeToRefs } from 'pinia';
      import { Refresh, Search } from '@element-plus/icons-vue'
      import { ElMessage, ElMessageBox } from 'element-plus'
      import { debounce } from "lodash"
      import {TenantStatus} from '@/common/enum';
  
      const router = useRouter();
      const tenantStore = useTenantStore();
      const {
        tenants, 
        total, 
        loading, 
        keySearch, 
        pageNumber, 
        pageSize, 
      } = storeToRefs(tenantStore);
      let debouncedFunction = null;
  
      const searchText = ref('');
  
      initData();
  
      onMounted(() => {
        console.log('onMounted');
      });
  
      async function initData() {
        await tenantStore.fetchList();
      }
  
      const btnAddOnClick = () => {
        router.push('/tenant/new');
      }
  
      const btnDeleteItemOnClick = (row) => {
        ElMessageBox.confirm(`Bạn có chắc chắn muốn xóa KH ${row.tenantName} ?`, 'Xác nhận', {
          confirmButtonText: 'Đồng ý',
          cancelButtonText: 'Hủy',
          type: 'warning',
        }).then(async () => {
          const isDeleted = await tenantStore.delete(row.tenantId);
          console.log(isDeleted)
          if (isDeleted) {
            ElMessage.success('Xóa thành công');
          } else {
            // ElMessage.error('Xóa thất bại');
          }
        });
      
      }
  
      async function searchTextOnInput() {
        if (!debouncedFunction) {
          debouncedFunction = debounce(() => {
            tenantStore.setKeySearch(searchText.value);
          }, 800);
        }
        debouncedFunction();
        
      }
  
      const btnViewItemOnClick = (row) => {
        router.push(`/tenant/view/${row.tenantId}`);
      }
  
      const btnEditItemOnClick = (row) => {
        router.push(`/tenant/edit/${row.tenantId}`);
      }
  
      async function btnRefreshOnClick() {
        await tenantStore.fetchList();
      }
  
      function tenantStatusFormatter(row, column, cellValue) {
        return TenantStatus[cellValue];
      }
  
  </script>
  
  <style scoped>
      .page__container{
        height: 100%;
        position: relative;
      }
  
      .el-table{
          color: #000;
          height: 100%;
      }
  
      .page__header {
        justify-content: space-between;
        align-items: center;
      }
  
      .table__container {
        overflow: auto;
      }
  
      .pagination {
        border-radius: 4px;
        display: flex;
        justify-content: flex-end;
        background-color: #fff;
        padding: 4px 10px 4px 10px;
      }
  
      :deep(.el-pagination__total) {
        position: absolute;
        left: 10px;
        font-weight: bold;
      }
  
      .search-container {
        justify-content: flex-end;
      }
  
      :deep(.el-button.is-circle){
        width: 32px;
        height: 32px !important;
      }
  
      :deep(.el-dropdown__caret-button){
        outline: unset;
      }
  
      :deep(.el-dropdown__caret-button:active){
        border-left: 1px solid #409eff;
      }
      
  
  </style>
  
  
  