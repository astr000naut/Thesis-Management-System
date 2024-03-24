<template>
  <div class="page__container flex-col rg-4">
    <div class="page__header flex-row">
      <h1 class="page__title" style="font-size: 24px;">Danh sách Khách hàng</h1>
      <el-button type="primary">Thêm mới</el-button>
      
    </div>
    <div class="table__container fl-1">
      <el-table :data="listItem" style="width: 100%">
        <el-table-column fixed prop="date" label="Date" width="150" />
        <el-table-column prop="name" label="Name"/>
        <el-table-column prop="state" label="State" width="120" />
        <el-table-column prop="city" label="City" width="120" />
        <el-table-column prop="address" label="Address" width="600" />
        <el-table-column prop="zip" label="Zip" width="120" />
        <el-table-column fixed="right" label="Operations" width="120">
          <template #default>
            <el-button link type="primary" size="small">
              Detail</el-button
            >
            <el-button link type="primary" size="small">Edit</el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>
    <div class="pagination">
      <el-pagination
        v-model:current-page="pageNumber"
        v-model:page-size="pageSize"
        :page-sizes="[10, 20, 50, 100]"
        :small="false"
        :disabled="false"
        :background="false"
        layout="total, sizes, prev, pager, next"
        :total="100"
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
      />
    </div>
  </div>
</template>


<script setup>
    import {useList} from '@/composables/useList';
    const {listItem, pageNumber, pageSize, total, loading} = useList();
    for (let i = 0; i < 5; i++) {
      listItem.value.push({
        date: '2016-05-02',
        name: 'John Smith',
        state: 'California',
        city: 'San Francisco',
        address: 'No. 189, Grove St, Los Angeles',
        zip: '123456'
      });
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
    

</style>


