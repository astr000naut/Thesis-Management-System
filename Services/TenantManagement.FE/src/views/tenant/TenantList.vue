<template>
  <div class="page__container flex-col rg-4">
    <router-view></router-view>
    <div class="page__header flex-row">
      <h1 class="page__title" style="font-size: 24px;">Danh sách Khách hàng</h1>
      <el-button type="primary" @click="btnAddOnClick">Thêm mới</el-button>
      
    </div>
    <div class="search-container flex-row al-center cg-2">
      <div class="reload-btn">
        <el-button :icon="Refresh" circle />
      </div>
      <div class="search-input">
        <el-input
          v-model="input2"
          style="width: 240px"
          placeholder="Tìm kiếm"
          :prefix-icon="Search"
        />
      </div>
    </div>
    <div class="table__container fl-1">
      <el-table :data="tenants" style="width: 100%">
        <el-table-column fixed prop="tenantCode" label="Mã khách hàng" width="150" />
        <el-table-column prop="tenantName" label="Tên khách hàng"/>
        <el-table-column prop="address" label="Địa chỉ" width="300" />
        <el-table-column prop="domain" label="Domain" width="300" />
        <el-table-column prop="state" label="Trạng thái" width="200" />
        <el-table-column fixed="right" label="Thao tác" width="120">
          <template #default>
            <el-button link type="primary" size="small">
              Xem</el-button
            >
            <el-button link type="primary" size="small">Sửa</el-button>
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
    import {onMounted} from 'vue';
    import { useTenantStore } from '@/stores';
    import { useRouter } from 'vue-router';
    import { storeToRefs } from 'pinia';
    import { Refresh, Search } from '@element-plus/icons-vue'

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

    initData();

    onMounted(() => {
      console.log('onMounted');
    });

    async function initData() {
      await tenantStore.reload();
    }

    const btnAddOnClick = () => {
      router.push('/tenant/new');
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
    

</style>


