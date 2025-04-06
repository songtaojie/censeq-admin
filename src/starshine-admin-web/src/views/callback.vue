<template>
  <div>
    <div desktop="12" tablet="8">
      <dl>
        <dt>authorize successful</dt>
        <dt>Your browser should be redirected soon</dt>
      </dl>
    </div>
  </div>
</template>
<script setup lang="ts" name="callback">
import { onBeforeMount} from 'vue';
import {authService} from '/@/oauth'

onBeforeMount(async () => {
  try {
      const url = window.location.href
      const user =  await authService.signinCallback(url)
      console.log('OIDC 登录成功:', user)
      // var curTime = new Date()
      // var expiredate = new Date(
      //   curTime.setSeconds(curTime.getSeconds() + user.expires_at)
      // )
      // window.localStorage.refreshtime = expiredate // 保存刷新时间，这里的和过期时间一致
      // var auth = {
      //   tokenExpire: expiredate,
      //   userId: user.profile.sub,
      //   userName: user.profile.preferred_username,
      //   nickName: user.profile.name
      // }
      // that.$store.commit('UPDATE_AUTH', auth)
      // that.$store.commit('UPDATE_USER', auth)
      // that.$store.commit('UPDATE_TOKEN', user.access_token)
      // window.localStorage.user = JSON.stringify(user)
      // that.$router.replace('/')
    } catch (err) {
      console.error('OIDC 登录失败:', err)
    }
})

</script>
