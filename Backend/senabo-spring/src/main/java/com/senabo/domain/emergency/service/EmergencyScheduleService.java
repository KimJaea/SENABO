package com.senabo.domain.emergency.service;

import com.senabo.config.firebase.FCMMessage;
import com.senabo.config.firebase.FCMService;
import com.senabo.domain.member.service.MemberService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.util.List;

@Slf4j
@Service
@RequiredArgsConstructor
public class EmergencyScheduleService {
    private final EmergencyService emergencyService;
    private final MemberService memberService;
    private final FCMService fcmService;

    // 오후 1시 - 오후 23시 : 1시간 마다 실행, 월 - 금
    @Scheduled(cron = "0 0 13-22 * * MON-FRI")
    public void scheduleWeekdayDayEmergency() {
        log.info("주중 낮 돌발 상황 스케줄러 실행");

        List<FCMMessage> emergencyList = memberService.findAllMemberNonComplete().stream()
                .filter(member -> member.getDeviceToken() != null && member.getId() != 16)
                .map(emergencyService::ramdomDayEmergency)
                .filter(fcm -> fcm.getMessage().getToken() != null)
                .toList();

        fcmService.sendFCM(emergencyList);
    }

    @Scheduled(cron = "0 0 0-3 * * MON-FRI")
    public void scheduleWeekdayEveningEmergency() {
        log.info("주중 저녁 돌발 상황 스케줄러 실행");

        List<FCMMessage> emergencyList = memberService.findAllMemberNonComplete().stream()
                .filter(member -> member.getDeviceToken() != null && member.getId() != 16)
                .map(emergencyService::ramdomEveningEmergency)
                .filter(fcm -> fcm.getMessage().getToken() != null)
                .toList();

        fcmService.sendFCM(emergencyList);
    }

    @Scheduled(cron = "0 0 5-7 * * SAT,SUN")
    public void scheduleWeekendEmergency() {
        log.info("주말 산책 돌발 상황 스케줄러 실행");

        List<FCMMessage> emergencyList = memberService.findAllMemberNonComplete().stream()
                .filter(member -> member.getDeviceToken() != null && member.getId() != 16)
                .map(emergencyService::ramdomWalkWeekendEmergency)
                .filter(fcm -> fcm.getMessage().getToken() != null)
                .toList();

        fcmService.sendFCM(emergencyList);
    }

    @Scheduled(cron = "0 0 19-23 * * SAT,SUN")
    public void scheduleBarkWeekendEmergency() {
        log.info("주말 짖음 돌발 상황 스케줄러 실행");

        List<FCMMessage> emergencyList = memberService.findAllMemberNonComplete().stream()
                .filter(member -> member.getDeviceToken() != null && member.getId() != 16)
                .map(emergencyService::ramdomBarkWeekendEmergency)
                .filter(fcm -> fcm.getMessage().getToken() != null)
                .toList();

        fcmService.sendFCM(emergencyList);
    }

    @Scheduled(cron = "0 0 5-8 * * SAT,SUN")
    public void scheduleVomitingWeekendEmergency() {
        log.info("주말 토 돌발 상황 스케줄러 실행");

        List<FCMMessage> emergencyList = memberService.findAllMemberNonComplete().stream()
                .filter(member -> member.getDeviceToken() != null && member.getId() != 16)
                .map(emergencyService::ramdomVomitingWeekendEmergency)
                .filter(fcm -> fcm.getMessage().getToken() != null)
                .toList();

        fcmService.sendFCM(emergencyList);
    }
}
